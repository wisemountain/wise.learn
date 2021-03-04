#pragma once

#include "Object.hpp"

#include <map>
#include <memory>
#include <string>

namespace scene
{
	class Scene
	{
	public: 
		Scene();

		~Scene(); 

		void Draw();

		template <typename T>
		std::shared_ptr<T> Create(const std::string& name)
		{
			return std::make_shared<T>(name);
		}

		void Place(ObjectPtr obj);

		void Remove(const std::string& name);

	private:
		using ObjectMap = std::map<std::string, ObjectPtr>;

	private:
		ObjectMap objects_;
	};
}