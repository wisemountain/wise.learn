# Learn OpenTK

WinForms에 맞춰진 라이브러리이다. WPF에서 사용하는 편법들은 다음과 같다. 
- WinForms Host를 사용하는 방법
- 프레임버퍼로 연결하는 방법 
- UserControl로 만드는 방법

그냥 생각하기에는 UserControl이 가장 나아보인다. 
프레임버퍼를 사용하는 게 가장 간결해 보인다. 

## Winforms host를 사용하는 방법 

https://www.gamedev.net/blogs/entry/2266764-gui-wpf-opengl-31/

https://www.codeproject.com/Articles/23736/Creating-OpenGL-Windows-in-WPF


## 프레임버퍼로 연결하는 방법 

https://github.com/freakinpenguin/O##penTK-WPF
여기에 OpenGLviaFramebuffer 예제가 있다. 

이미지가 이상해서 그대로 복사하고 namespace만 변경했다. 
동작하긴 한다. 


# 작은 엔진 만들기 

딱 서버 프로그래머에 맞는 시각화 기능을 갖춘 엔진을 만든다. 
별도 프로젝트로 정리하기 전에 먼저 충분히 연습을 한다. 

## Vertex와 Mesh 

- Position, Normal, Color, Uv
- 애님 기능은 나중에 
- 버퍼도 동적으로 여러 옵션 처리가 가능하도록 하는 건 나중에 

## Scene 

- Node로 Mesh / Material 감싸기

## 카메라 

- 기본 카메라 만듦 

## Shader

- 작업 중 
- 이상한 오류 나와서 보는 중 
- 왜 그럴까? 


## Picking 


마우스 위치로 가는 카메라 위치에서 시작하는 레이를 만든다. 
Projection / View를 역으로 변환해서 원래 3D 공간 상의 위치를 찾는다. 

마우스 위치는 카메라와 프로젝션 변환을 거친 화면 공간 상의 위치이기 때문에 역변환을 하면 된다. 

이후에는 월드 공간 상의 오브젝트와 체크를 하면 된다. 
매시가 크다면 미리 빠르게 처리 가능한 단위로 분할해 두지 않으면 느릴 수 있다. 

```c#
private Vector3 UnProjectPos(Vector3 mouse, Matrix4 projection, Matrix4 view, Size viewport)
{
	Vector4 vec;

	vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
	vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
	vec.Z = mouse.Z;
	vec.W = 1.0f;

	Matrix4 viewInv = Matrix4.Invert(view);
	Matrix4 projInv = Matrix4.Invert(projection);

	Vector4.Transform(ref vec, ref projInv, out vec);
	Vector4.Transform(ref vec, ref viewInv, out vec);

	if (vec.W > 0.000001f || vec.W < -0.000001f)
	{
		vec.X /= vec.W;
		vec.Y /= vec.W;
		vec.Z /= vec.W;
	}

	return vec.Xyz;
} 
```		

NDC 장치 좌표로 마우스 좌표를 옮긴다. 다 그려진 후이기 때문에 이 좌표를 사용하여 
역변환을 projInv, viewInv에 대해 진행하면 월드 좌표가 나온다. 

W는 깊이 성분이므로 이 값으로 위치가 주어진다. 

UnPorjectPos(x, y, 0)이 시작 점이고 UnProjectPos(x, y, 1)이 끝점이다. 


# Bullet 테스트 

BulletSharp으로 진행. 
 
- Picking
- Collision Query
- json scene 


### 따라하기 

http://www.opengl-tutorial.org/kr/miscellaneous/clicking-on-objects/picking-with-a-physics-library/










## 피킹 

TriangleMesh
- 정적으로 처리. 미리 버텍스 변환해서 삼각형들 추가해야 함 
- 뷰포트 크기를 정확하게 얻어서 Unproject 함수를 사용하면 됨 



