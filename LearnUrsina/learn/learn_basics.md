# learn basics 

ursina는 panda3d를 기반으로 한다고 들었다. 쉽게 사용할 수 있도록 모든 앱에 필요한 초기 설정 값들을 내부에 포함하고 있다. 
working defaults를 제공하는 방법을 사용한다. 이런 경우 세부적인 제어가 어려울 수도 있기 때문에 빨리 시작하고 
크게 좌절하는 경우도 많이 있다. 

소스를 포함하는 develop 설정으로 설치했기 때문에 필요하면 소스를 수정할 수도 있다. 
괜찮으면 fork해서 개선하면서 사용한다. 

## 폴더 둘러 보기 

ursina 폴더들 돌아다니면서 본다. 

### samples 

* column_graph.py : 그래프, GUI 사용법을 함께 보여주는 것이기도 하다. 
* example_game.py : 뭐지? 파일 이름이 게임이면 안 된다. 단순 GUI 예제 
* heksekraft.py : 엥? 
* minecraft_clone.py : 이쪽에 관심이 있는 듯. 
* options_menu.py : 버튼들 
* platformer.py : 헉!
* small_example_game.py : 작고 허접한 점프 이동 게임 
* terrain_mesh.py : 이건 괜찮다. 
* tic_tac_toe.py 
* undo.py 
* world_grid.py : 이건 유용하다. 

### ursina

* editor 
* fonts 
* models 
* prefabs
* scripts 
* shaders
  * 문자열로 포함. 
  * python 파일이다
  * 아주 오래된 130 버전의 쉐이더 

최상위 폴더에 public 클래스들과 함수를 갖고 있다. 

prefabs는 재사용 가능한 기능들을 둔다. 
scripts는 애셋 처리와 같은 작은 툴 프로그램들이다. 


## world_grid.py 샘플 분석 

hello world에 해당하는 박스나 구 하나 그리는 앱을 만든다. 

```python
from ursina import *


app = Ursina()

r = 8
for i in range(1, r):
    t = i/r
    s = 4*i
    print(s)
    grid = Entity(model=Grid(s,s), scale=s, color=color.color(0,0,.8,lerp(.8,0,t)), rotation_x=90, position=(-s/2, i/1000, -s/2))
    subgrid = duplicate(grid)
    subgrid.model = Grid(s*4, s*4)
    subgrid.color = color.color(0,0,.4,lerp(.8,0,t))
    EditorCamera()

app.run()
```

### app = Ursina() 

자두라는 얘기가 있는 데 확실히 모르지만 의미를 부여해야 기억하기 쉽기 때문에 자두라고 하자. 

Ursina() 엔진 객체를 만든다. 

grid를 Entity()로 만든다. app에 등록하는 코드가 안 보이므로 Entity 생성시 
Ursina() 객체에 접근하여 등록한다. 없어도 죽지는 않고, 안 보이기만 한다. 


### grid = Entity(...)

언리얼의 액터, 유니티의 GameObject와 같은 수준의 엔진 내 보이는 것들을 추상화 한 것이다. 
이름이 있는 아규먼트로 model, scale, color, rotation_x, position을 갖는다. 


### EditorCamera() 

에디터 카메라 설정이다. 


## 연습 

* 구모델 띄우기 

### Step 1. Minimal Application

```python
from ursina import * 

app = Ursina()

EditorCamera()

app.run()
```

### Step 2. Sphere 

