# flatbuffer 파서 생성기 

단계적으로 진행한다. 토큰들부터 파싱하고 점점 복잡한 문법을 추가한다. 

## tokens

regular expression을 사용한다. 따라서, lark의 RE 문법을 알아야 한다. 
연습을 작성하고 확인하고 다음 단계로 간다. 


## grammar review

https://lark-parser.readthedocs.io/en/latest/grammar/

- rule (production rule)
- terminals

- EBNF 

```
a b? c => (a c | a b c) 
```

```
a: b*  => a: _b_tag 
          _b_tag: (_b_tag b)?
```
위는  , b, bb, bbb 등으로 확장된다. 

```
rule: <EBNF EXPRESSION> 
    | etc. 
TERM: <EBNF EXPRESSION> 
```


### terminals 

```
<NAME> [. <priority> ] : <literals-and-or-terminals>
```

- uppercase
- "string" 
- /regular expression+/
- "case-insensitive string"i
- /re with flags/imulx 
- Literal range: "a".."z", "1".."9"

Terminals also support grammar operators, such as `|`, `+`, `*`, and `?`

```c++
IF: "if"
INTEGER : /[0-9]+/
INTEGER2 : ("0".."9")+          //# Same as INTEGER
DECIMAL.2: INTEGER? "." INTEGER  //# Will be matched before INTEGER
WHITESPACE: (" " | /\t/ )+
SQL_SELECT: "select"i
```

### regular expressions & ambiguity 

Each terminal is eventually compiled to a *regular expression*.

```
A1: "a"|"b"
A1: /a|b/
```
둘이 같다. 

```
start       : (A | B)+
A           : "a" | "b"
B           : "b"
```
위에는 모호함이 있다. 두 가지로 해석될 수 있다. rule을 사용하면 ambiguity를 보여준다. 

### rules 

```
<name> : <items-to-match> [-> <alias>]
    | ...
```

각 항목은 다음과 같다: 
- rule 
- TERMINAL 
- "string literal" or /regex literal/
- (item item ...) Group
- [item item ...] Maybe. Same as (item item ...)?
- item?
- item* 
- item+
- item~n
- item ~n..m

### directives

%ignore <TERMINAL> 

```python
%ignore " "

COMMENT: "#" /[^\n]/*
%ignore COMMEN
```

%import : lark grammar에서 로딩 

### common.*

C:\app\python37\Lib\site-packages\lark

common.lark

단말에 대한 좋은 예시이다. 많은 것들을 포함하고 있다. 


## 진행 

terminal부터 상위 규칙으로 차례로 하나씩 확인하면서 learn/flat_re.py로 진행했다. 
생각보다 수월하게 진행되었다. 

- vector_type 추가 
  - 의미를 부여하기 편하도록 문법을 개선한다. 

## 파스트리 

 Tree(
     type_decl, 
     [Token(TABLE, 'table'), 
        Tree(type_decl_name, [Token(IDENTIFIER, 'Weapon')]), 
        Tree(metadata, []), 
        Token(OPEN_BLOCK, '{'), 
            Tree(field_decl, 
                [Tree(field_name, [Token(IDENTIFIER, 'name')]), 
                    Token(COLON, ':'), 
                    Tree(type, 
                        [Tree(builtin_type, [Token(STRING, 'string')])]), 
                        Token(SEMICOLON, ';')]), 
            Tree(field_decl, 
                [Tree(field_name, [Token(IDENTIFIER, 'damage')]), 
                    Token(COLON, ':'), 
                    Tree(type, 
                        [Tree(builtin_type, [Token(SHORT, 'short')])]), 
                        Token(SEMICOLON, ';')]), 
        Token(CLOSE_BLOCK, '}')
     ]
)

트리 구조로 되어 있다. Tree/Token을 순회하면서 의미를 붙여야 한다. 

## Transformer

lark에서 semantic analysis를 진행하고 semantic action을 실행하는 구조가 transformer이다. 

```python
class MonsterTree(Transformer):

    def include_decl(self, s):
        for c in s:
            print("include => ", c)

    def namespace_decl(self, s):
        for c in s:
            print("type: ", c.type, "value: ", c.value, "\n")

    def type_decl(self, s): 
        for c in s:
            if type(c) is Tree:
                print("tree: ", c.data, "\n")
            elif type(c) is Token:
                print("token: ", c.value, "\n")
```

Transformer는 tree visitor이다. 여기에 문맥(상태)을 기록하고 조건에 따라 실행(semantic action)한다. 

