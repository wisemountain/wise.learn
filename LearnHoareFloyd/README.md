# Learn Hoard-Floyd Logic

Background reading on Hoare Logic
 - Mike Gordon

 https://www.cl.cam.ac.uk/archive/mjcg/HoareLogic/

위 강의 노트로 진행한다. 다 읽고 discipline을 만든다. 

## Introduction

partial correctness spec. {P} C {Q}

C : 
 - assignment 
 - if 
 - while 
 - sequence. C1; C2; ... ; Cn

```bnf
<command>
::= <variable>:=<term>
| <command>; . . . ; <command>
| IF <statement> THEN <command> ELSE <command>
| WHILE <statement> DO <command>
```

partial since there is no guarantee of termination. 

[P] C [Q] : total correctness spec. 

Collatz conjecture: 
```c++
while X>1 {
    if ODD(X) {
        X := (3×X)+1 
    }
    else 
    {
        X := X DIV 2
    }
}
```
위 문장이 끝나는 지 증명되지 않았다. 놀랍군. 
conjecture는 x가 3x + 1 번 만에 1이 된다는 것. 


## Examples

{X=1} Y:=X {Y=1}   : true
{X=1} Y:=X {Y=2}   : false (Post condition is false)
{X=x and Y=y} R:=X; X:=Y; Y:=R {X=y and Y=x}
{X=x and Y=y} X:=Y; Y:=X {X=y and Y=y}  : false
{T}C{Q}
{P}C{T}

```c++
{T}
R = X; 
Q = 0;
while (Y <= R) 
{
    R = R - Y; 
    Q = Q + 1;
}
{R < Y and X = R + (Y * Q)>}
```
위는 참이다. 수론 문제를 푸는 듯. (위는 계산 문제이다)


## Hoare Logic


|- S : S has a proof. theorem. 

|- S1, ..., |- Sn
 ----------------
   |- S 

the conclusion |- S may be deduced from |- S1, ..., |- Sn, which are hypotheses of the rule. 


## assignment 

P[E/V] : V를 모두 E로 교체. P with E for V. 

(X+1 > X) [Y+Z/X] = (Y+Z+1 > Y+Z)

V[E/V] = E : cancellation law


|- {P[E/V]} V := E {P}

처음에 위 axiom을 보면 뭔소린가 싶다. 
단순한 P를 생각해 보자.  P : V > 3 => P[E/V] => E > 3 => V := E => V > 3 => P
원래 axiom이 나온다. 


⊢ {X + 1 = n + 1} X := X + 1 {X = n + 1}

위에서 P는 X=n+1이고 E는 X+1이다. X가 V에 해당한다. 
대입을 적용하면 P[E/V]는 P[E/X]이고 X+1=n+1이 된다. 

P를 만족하도록 P[E/V]에 해당하는 할당이 C에서 이루어진다. 

실제 테스트를 구성할 때 어떻게 해야할 지 봐야 하다. 

Floyd assignment axiom: 

⊢ {P } V :=E {∃v. (V = E[v/V ]) ∧ P [v/V ]}
Where v is a new variable (i.e. doesn’t equal V or occur in P or E)

```
⊢ {X=1} X:=X+1 {∃v. X = X+1[v/X] ∧ X=1[v/X]}
⊢ {X=1} X:=X+1 {∃v. X = v + 1 ∧ v = 1}
⊢ {X=1} X:=X+1 {∃v. X = 1 + 1 ∧ v = 1}
⊢ {X=1} X:=X+1 {X = 1 + 1 ∧ ∃v. v = 1}
⊢ {X=1} X:=X+1 {X = 2 ∧ T}
⊢ {X=1} X:=X+1 {X = 2}
```

weakest liberal precondtion, strongest post condition. 4장에서 설명한다. 


## Precondition strengthening 

 |- P => P', |- {P'} C {Q}
 -------------------------
      |- {P} C {Q}


## Postcondition weakening 

 |- {P} C {Q'}, |- Q' => Q
 -------------------------
      |- {P} C {Q}


## Specificatin conjunction and disjunction

Specification conjunction

 ⊢ {P1} C {Q1}, ⊢ {P2} C {Q2}
 -----------------------------
 ⊢ {P1 ∧ P2} C {Q1 ∧ Q2}

Specification disjunction

 ⊢ {P1} C {Q1}, ⊢ {P2} C {Q2}
 -----------------------------
 ⊢ {P1 ∨ P2} C {Q1 ∨ Q2}


## The sequencing rule 

The sequencing rule

 ⊢ {P} C1 {Q}, ⊢ {Q} C2 {R}
 --------------------------
   ⊢ {P} C1;C2 {R}


여럿 버전 

The derived sequencing rule
                ⊢ P ⇒ P1
 ⊢ {P1} C1 {Q1}, ⊢ Q1 ⇒ P2
 ⊢ {P2} C2 {Q2}, ⊢ Q2 ⇒ P3
 . .
 . .
 . .
 ⊢ {Pn} Cn {Qn}, ⊢ Qn ⇒ Q
 ---------------------------
 ⊢ {P} C1; . . . ; Cn {Q}


## The conditional rule 

The conditional rule

 ⊢ {P ∧ S} C1 {Q}, ⊢ {P ∧ ¬S} C2 {Q}
 ------------------------------------
   ⊢ {P} IF S THEN C1 ELSE C2 {Q}

C1을 실행하거나 C2를 실행하건 Q를 만족하면 결과를 만족한다. 

커버리지를 할 때는 S, !S를 모두 테스트. 
Q는 S일 경우랑, !S일 경우 달라져야 한다. 


## The while rule

The WHILE-rule

    ⊢ {P ∧ S} C {P}
----------------------------
⊢ {P} WHILE S DO C {P ∧ ¬S}


## The for rule 

for v := E1 until E2 do C


The FOR-axiom

⊢ {P ∧ (E2 < E1)} FOR V := E1 UNTIL E2 DO C {P}


Wickerson’s FOR-rule

  ⊢ P ⇒ R[E1/V ], ⊢ R ∧ V >E2 ⇒ Q, ⊢ {R ∧ V ≤E2} C {R[V +1/V ]}
 ---------------------------------------------------------
           ⊢ {P} FOR V := E1 UNTIL E2 DO C {Q}

where neither V , nor any variable occurring in E1 or E2, is assigned to in
the command C.


Tennent’s FOR-rule

    ⊢ {P[V −1/V ] ∧ (E1 ≤ V ) ∧ (V ≤ E2)} C {P}
 -----------------------------------------------------------------
  ⊢ {P[E1−1/V ]∧(E1−1≤E2)} FOR V := E1 UNTIL E2 DO C {P[E2/V ]}

where neither V , nor any variable occurring in E1 or E2, is assigned to in
the command C.



## array 

The array assignment axiom

⊢ {P[A{E1←E2}/A]} A(E1):=E2 {P}


The array axioms

⊢ A{E1←E2}(E1) = E2

E1 != E3 => ⊢ A{E1←E2}(E3) = A(E3)

A{E1 <- E2}는 A(E1)에 E2를 할당하는 (거기만 할당하는) 연산이다. 


## mechanizing program verification 

verification 자동화에 대해 논의한다. 여기서부터 갈린다. 


## goal oriented methods



# 아이디어

- 함수 단위 검증 
- 호출 단위 검증 
  - permutation 
- 상태 단위 검증
  - invariance


