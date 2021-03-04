#pragma once

#include <glm/vec3.hpp>
#include <memory>

namespace scene
{
	class Scene;

	// scene에 배치되는 오브젝트들의 기반 클래스
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