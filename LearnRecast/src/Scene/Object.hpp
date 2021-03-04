#pragma once

#include <glm/vec3.hpp>
#include <memory>

namespace scene
{
	class Scene;

	// scene�� ��ġ�Ǵ� ������Ʈ���� ��� Ŭ����
	class Object
	{
	public: 
		Object(Scene* scene);

		void Draw();

	private: 
		glm::vec3 pos_;
		glm::vec3 rot_;
	};

	using ObjectPtr = std::shared_ptr<Object>;
}