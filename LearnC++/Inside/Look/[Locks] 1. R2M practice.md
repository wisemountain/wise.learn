# [Locks] R2M practice

R2M에서 사용하고 있는 락 구조를 살피면서 여러 아이디어를 다방면으로 생각해 보고
개별 아이디어를 별도로 살펴본다. 

## 핸들러 

핸들러는 패킷과 타이머로 발생하는 시스템 내의 이벤트를 처리하는 처리기이다. 
R2M에서 CTroc의 하위 클래스들의 Proc 함수들이 핸들러이다. 

핸들러를 명시적으로 구분하는 것은 내부 구조와 이벤트에 따른 처리를 분리하고 
이벤트의 처리가 DBMS에서 트랜잭션과 같은 단위의 쓰레드 상의 실행에 대응한다고 
볼 수 있기 때문이다. TR이 Transaction의 약자이고 이와 같이 생각하여 
shared state multithreading에서 오브젝트 접근과 처리를 했기 때문에 비교적 
명확한 구조를 R2M에서 갖고 있는 이유 중 하나라고 본다. 

타이머와 패킷 이외에는 쓰레드에서 처리를 시작하는 코드가 없다는 전제를 
만족한다고 보면 처리 구조가 단순해진다. 

### 핸들러의 단일 오브젝트 락 처리 구조 

uint32_t CTrocChase::Proc(TSession& pSes, TFbject& pFbj, const void* pData, uint32_t pLng) const

CTrocAttack.cpp 파일에 있는 CTrocChase::Proc 핸들러로 이 처리기는 
처음에는 CTrocAttack이었으나 오토 플레이 구현을 위해 CTrocChase로 변경되었다. 

- TFbject (FrameworkObject)에서 CPc로 변환한다. 

- CPc::ResetLogout()을 호출한다. 
    - 락 없이 __mLogoutTick을 수정한다. 

- CPc::ResetUntouchable()
    - 락 없이 	__mUntouchableLimitTick과 __mPcFlag를 수정한다. 
	
- CPc::IsObserver() 
    - 락 없이 __mGbjFlag를 참조한다. 

- CChar::Idle() 
    - 내부 락을 사용한다. 
    - 락을 풀고 Transit()을 호출하고 
        - TransitFsm()에서 내부 락을 사용한다. 

- CChar::Chase() 
    - 락을 사용하지 않으면서 처리한다. 
    - 락을 사용하는 Idle(), Transit() 함수를 호출한다. 

이와 같은 처리 흐름은 호출과 처리 구조에서 락을 사용하는 함수와 하지 않는 함수를 구분해야 하고 
단일 오브젝트 접근에 대해서도 일관된 처리 구조를 갖기가 어렵다. 

읽기 함수나 변수 업데이트에 락을 사용하지 않으면 버그가 나올 가능성이 상존한다. 
따라서, 이 부분을 명확한 스킴으로 통합하는 것이 필요하다. 


### 핸들러의 다중 오브젝트 락 처리 구조 

uint32_t CTrocUseSkillReq::Proc(TSession& pSes, TFbject& pFbj, const void* pData, uint32_t pLng) const

스킬 사용은 대상이 항상 있으므로 여러 오브젝트에 대해 동작한다. 

- CSkillController::UseSkill() 

    - WithLock() 함수로 Fsm 상태를 변경 
    - Target의 정보에 접근하여 스킬 시전의 유효성 검사 
        - Target의 정보를 읽을 때 거의 락을 하지 않음 

    - CPc::AttackWeapon() 호출 
        - 

데드락을 방지하기 위한 코드들로 보이며 다른 오브젝트에 대한 락 접근 시 현재 락을 풀면 
대부분 해결될 것으로 보인다. 트랜잭션을 처리하는 코드는 아니다. 

Controller는 Owner의 락을 사용한다. 

### 엔티티 함수 검토 1

```c++
uint32_t CPc::ItemUnequip(CEquipment::ESlot pEquipSlot, bool pIsForce, bool pIsSendInfo)
{
	FNL_ASSERT(IsAccessible(), "Invalid!");
	FNL_ASSERT(IsLogin(), "Invalid!");
	FNL_ASSERT(CHECK_FBS_ENUM_RANGE(pEquipSlot, CEquipment::ESlot), "Invalid!");

	uint32_t aErrorCode{};

	if (false == pIsForce)
	{
		aErrorCode = Get<CInventoryController>()->CheckInventoryAccessible();   // 함수가 thread-safe 하다. read / 단일 오브젝트
		if (aErrorCode)
		{
			return aErrorCode;
		}
	}

    // write access
    {
        Writer w( this );

        const CEquipStatus					aEquipStatus{ __mEquipment.GetEquipStatus(pEquipSlot) };
        const CItemInventory::TInfo* const	aInfo{ aEquipStatus.GetItemInfo() };
        if (&CItemInventory::TInfo::GetDefault() == aInfo)
        {
            return eErrNoItemInvalid;
        }

        aErrorCode = __mEquipment.Unequip(pEquipSlot);
        if (aErrorCode)
        {
            return aErrorCode;
        }
    }

	ApplyEquipment(false, aInfo); // writer / single 

	CSqlPcItem& aSql = static_cast<CSqlsGame*>(FnlFw::CSqlPool::This().GetSqlsGame())->mSqlPcItem;

	int32_t aSqlErrorCode = aSql.PopEquip(_mInfo.mNo, FnlApi::UnderType(pEquipSlot));
	if (0 != aSqlErrorCode)
	{
		if (aSqlErrorCode < 0)
		{
			aSql.LogAndReset(__FUNCTIONW__);
		}

		CONSOLE_SYS(ELogLv::Fatal, eErrNoSqlFailExec, "CSqlPcItem::PopEquip(%lld:%s)(%d)(%d)"
					, _mInfo.mNo, _mInfo.mName, pEquipSlot
					, aSqlErrorCode);

		return eErrNoSqlFailExec;
	}

	if (pIsSendInfo)
	{
		bool aSendInfo[eSendInfoCnt]{};
		aSendInfo[eSendInfoAbility] = true;
		aSendInfo[eSendInfoSpeed] = true;
		ProcessSendInfo(aSendInfo);
	}

	if (IsDisplayEquipment(pEquipSlot)) // reader / single 함수로 분리 
	{
		FnlFw::CSender aSender;
		FnlFw::MakeTr<Field::FBItemUnequipAck>(aSender, Field::CreateFBItemUnequipAck
											   , GetUnique().GetRaw(), pEquipSlot, aInfo->GetFakeID());
		SendToNeighbor(aSender);
	}
	else
	{
		FnlFw::CSender aSender;
		SendOnlyOne<Field::FBItemUnequipAck>(aSender, Field::CreateFBItemUnequipAck
											 , GetUnique().GetRaw(), pEquipSlot, aInfo->GetFakeID());
	}

	return aErrorCode;
}
```



## 아이디어들 

- 핸들러 

- 대상의 개수에 따른 분류 
    - 단일 오브젝트 
    - 다중 오브젝트 
        - 순차 접근 (콜 그래프와 같은 형태) 
        - 트랜잭션 
    
- 접근 
    - 읽기
    - 쓰기 

- recursive_shared_mutex 
    - 읽기 효율을 높이기 위함
    - recursive로 만들어 단일 오브젝트 내 락 처리를 수월하게 만들기 

    - Claim: 단일 오브젝트에 대한 처리는 오브젝트 내의 recursive_shared_mutex로 
      구현하면 읽기 효율은 유지하면서 안전하고 일관되게 처리할 수 있다. 

        - 모든 public 함수에 recursive_shared_mutex를 읽기 / 쓰기 모드로 지정 

    - Claim: 재귀 호출과 락 reentrance를 효율적으로 처리해야 한다. 
        - 이전 락의 unlock으로 대부분 처리된다. 

- lock_tracer 
    - lock_trace_guard( tracer, reader(lock) )
    - lock_trace_guard( tracer, writer(lock) )


- 단일 오브젝트 핸들러에서 시작할 경우 쓰레드별 한 시점에 걸리는 락은 하나로 한다. 
    - 오브젝트 내에서 다른 오브젝트 호출을 할 경우 이전 락을 풀고 접근한다. 

- 엔티티의 함수 구분 

    - 핸들러 함수 
        - 다른 오브젝트 함수 호출 
        - 자신의 함수만 호출 

    - 데이터 함수 
        - 데이터 접근과 룰 적용 


- 가장 빠른 접근은 핸들러에서 필요한 락들을 잡고 처리하는 것 

    - 각 엔티티는 assert로만 검증한다. 
        - RX_ENTITY_LOCK_FMT( !IsLockedShared(), desc )
        - RX_ENTITY_LOCK_FMT( !IsLockedUnique(), desc )
        - desc에 __FILE__, __LINE__, __FUNCTION__ 포함 
        - this 포인터 포함. 
        - 에러 로깅과 연결 끊기. 

    - CLockable의 접근 
        - 각 오브젝트에 recursive_shared_mutex를 두고 처리 
        - assert 가능하게 함 
        - 핸들러 함수들에서 락을 잡고 진행 
        - 세부적인 제어가 가능하고, 엔티티 자체는 락만 갖고 있는다. 


- 가장 오류가 적은 접근은 각 엔티티 내부에서 thread-safe하게 처리하는 것이다. 

    - 검증을 위해 락을 TLS로 트래킹한다. 
    - 필요하면 unlock, upgrade 할 수 있다. 
    - CLockable을 기반 클래스로 하고, CLockTracer를 둔다. 
    - ILockable 인터페이스를 별도로 두어 Controller와 같은 컴포넌트는 CLockable을 락을 사용하게 한다. 
        - Entity의 락을 돌려준다. 


- CMutex와 CSharedMutex 구현

    - 디버깅을 위한 몇 가지 기능을 갖고 있다. 
    - 


스킴 1: 

- 쓰레드 별로 한번에 하나의 락만 잡도록 하고
- recursive_shared_mutex를 사용하게 하고
- Entity 내부에서만 락을 잡도록 한다. 

이렇게 할 경우 트랜잭션 처리는 unique를 락을 대상 오브젝트들에 대해 
미리 잡고 처리하면 된다.

recursive이므로 처리가 되고 shared lock 요청은 지나가야 한다. 
- 상속 받아서 이미 xlock을 갖고 있으면 slock을 얻을 수 있게 한다. 
    - 이건 owner 체크 만으로 가능하다. 
- lock advance (slock -> xlock)은 데드락이 있을 수 있다고 한다. 



















