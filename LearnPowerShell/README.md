# Windows PowerShell 

이름이 PowerShell이고 실제로 강력해 보이기는 하지만 여러 복잡한 오브젝트와 코딩으로 이루어져있어 아무리 윈도우 NT가 VMS의 영향을 
강하게 받았다고는 하나 쉘 스크립트까지 꼭 그렇게 해야 하는 생각이 들고 유닉스로 쉘스크립트를 배웠던 입장에서는 달갑지도 않아 지금까지 
등한시하고 있었다. 

모든 도구는 알고 익숙해져야 정이 들고 유용해지고, 실제 툴로 사용해야 할 필요가 있어 이제는 익히려고 한다. 

과제 위주로 정리하면서 추가한다. 

- 파워쉘 튜토리얼 
- visual studio code의 PowerShell 확장 
- 프롬프트 바꾸기 
- 환경 변수 설정 
- grep 


## 튜토리얼 

기본 문법과 개념에 대한 튜토리얼을 따라간다. 


### 초간단 

https://blog.netwrix.com/2018/02/21/windows-powershell-scripting-tutorial-for-beginners/

powershell_ise.exe 
- 개발환경이다. 
- 함수들 도움말이 있고, 디버깅 기능도 있다. 


- .ps1 파일로 저장
- 우클릭으로 Run with PowerShell로 실행 
- 실행 권한 
  - Get-ExecutionPolicy 
  - Set-ExecutionPolicy 

  - Restricted, AllSigned, RemoteSigned, Unrestricted 

- cmdlets
  - predefined function 
  - Get, Set, Start, Stop, Out, New 

- Get-Process | Get-Member
- (Get-Process).Handles 
- (Get-Service).DisplayName 

마음에 들기 시작했다. 

- Get-Help Get-Service 

- cmdlet - 
  - 파라미터 목록 표시 

ISE가 좋다. Auto completion 기능을 제공하므로 편하게 작업할 수 있다. 

- Get-Service -Name R* 
- Get-Service -Name 
  - 여기서 전체 서비스 목록을 보여준다. 선택할 수 있다. 


- 스크립트 주석 
  - # 
  - <#   ... #>

- Get-Service | Sort-Object -property Status
- "Hello, World" | Out-File C:\ps\test.txt

- Get-Service | WHERE { $_.status -eq "Running" } | SELECT displayname
  - 모든 실행 중인 프로세스 이름을 출력 

```powershell
Get-Service | WHERE { $_.Status -eq "Running" } | select displayname | Sort-Object -property DisplayName
```

## 파워쉘 확장 

함수 도움말을 보여주고, 파워쉘 창도 띄워준다. 
아주 많은 함수가 있다는 정도? 


## 프롬프트 바꾸기 

가장 초보적인 연습이다. 

https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_prompts?view=powershell-7

- Prompt function 
- PowerShell profile 

```PowerShell 
function Prompt { "$ " }

Get-Command Prompt 

(Get-Command Prompt).ScriptBlock 
```

- Function: drive 
- cmdlet 
- $profile 명령으로 프로파일 생성 
  - C:\Users\User\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1

```PowerShell
function prompt {
  $p = Split-Path -leaf -path (Get-Location)
  "$p> "
}
```

Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

위 권한 설정을 해줘야 파워쉘 스크립트를 실행할 수 있다. 

https://superuser.com/questions/446827/configure-the-windows-powershell-to-display-only-the-current-folder-name-in-the



## 환경 변수 설정 

개발 환경을 정리할 때 라이브러리 홈을 환경변수로 갖고 있으면 편리할 때가 많다. 장비가 바뀌거나 여러 사람이 참여하는 프로젝트의 경우 프로그래머마다 매번 설정을 해야 하므로 귀찮기도 하고 비효율적이기도 하다. 

프로그래밍과 기계학습 모두 사람의 일을 덜어주고, 사람이 어려워 하는 일을 하는 게 기본적인 목표이므로 항상 그런 일에 관심을 갖고 살펴야 
프로그래머로서도 좋고 사람들도 좋다. 

작은 일이지만 소중한 일이다. 

스크립트를 만들어 환경 변수를 자동으로 등록할 수 있게 하는 것이 목표이다. 


- Set-Location Env: 
  - Env: 드라이브로 이동 

- Get-ChildItem 
- Get-Item Path

- System.Collections.DicionaryEntry class 
  - 이름이 키

- Get-Member cmdlet 
  - display methods and properties 

```PowerShell
 Get-Item -Path Env:* | Get-Member
 ```

 ```
    TypeName: System.Collections.DictionaryEntry

Name          MemberType    Definition
----          ----------    ----------
Name          AliasProperty Name = Key
Equals        Method        bool Equals(System.Object obj)
GetHashCode   Method        int GetHashCode()
GetType       Method        type GetType()
ToString      Method        string ToString()
PSDrive       NoteProperty  PSDriveInfo PSDrive=Env
PSIsContainer NoteProperty  bool PSIsContainer=False
PSPath        NoteProperty  string PSPath=Microsoft.PowerShell.Core\Environment::COMPUTERNAME
PSProvider    NoteProperty  ProviderInfo PSProvider=Microsoft.PowerShell.Core\Environment
Key           Property      System.Object Key {get;set;}
Value         Property      System.Object Value {get;set;}
```


- expression parser 
  - $로 시작 
  - $Env:Path 
  - $Env:windir

- 설정 
  - $Env:GLFW = value 
  - $Env:Path += ";c:\temp"


현재 사용자, 전체 호스트에 적용 
```PowerShell
Add-Content -Path $Profile.CurrentUserAllHosts -Value '$Env:Path += ";C:\Temp"'
```
여기가 하려는 것이었다. 
위의 효과는 다시 시작해야 나타난다. 바로 적용하려면 어떻게 해야 하는가? 

```powershell
[Environment]::SetEnvironmentVariable
     ("INCLUDE", $env:INCLUDE, [System.EnvironmentVariableTarget]::User)
```     

- 사용자 경로 변수 변경 

```powershell
[Environment]::SetEnvironmentVariable("Path", [Environment]::GetEnvironmentVariable("Path", "User") + ";C:\bin", "User")
```

SetEnviromentVariable 함수 
- 환경 변수 이름 (문자열) 
- 값 (문자열) 
- 범위 ("User", "Machine")


## Grep 

- Select-String -pattern Pattern

```powershell
$event | select-string -inputobject {$_.message} -pattern "오프라인"
```

https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/select-string?view=powershell-7


```powershell 
Get-Command | Out-File -FilePath .\Command.txt
Select-String -Path .\Command.txt -Pattern 'Get', 'Set'  -NotMatch
```



