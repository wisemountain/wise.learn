#include <iostream>
#include "monster_generated.h"

using namespace MyGame::Sample;
using namespace flatbuffers;

void tutorial()
{
	FlatBufferBuilder fbb(1024);

	uint8_t* buf = 0;
	std::size_t size = 0;

	// write
	{
		auto sword = CreateWeapon(
			fbb,
			fbb.CreateString("Sword"),
			3);

		auto axe = CreateWeapon(
			fbb,
			fbb.CreateString("Axe"),
			5);

		// Serialize a name for our monster, called "Orc".
		auto name = fbb.CreateString("Orc");

		unsigned char treasure[] = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		auto inventory = fbb.CreateVector(treasure, 10);

		std::vector<flatbuffers::Offset<Weapon>> weapons_vector;
		weapons_vector.push_back(sword);
		weapons_vector.push_back(axe);
		auto weapons = fbb.CreateVector(weapons_vector);

		Vec3 points[] = { Vec3(1.0f, 2.0f, 3.0f), Vec3(4.0f, 5.0f, 6.0f) };
		auto path = fbb.CreateVectorOfStructs(points, 2);

		// Create the position struct
		auto position = Vec3(1.0f, 2.0f, 3.0f);

		// Set his hit points to 300 and his mana to 150.
		int hp = 300;
		int mana = 150;
		// Finally, create the monster using the `CreateMonster` helper function
		// to set all fields.
		auto orc = CreateMonster(fbb, &position, mana, hp, name, inventory,
			Color_Red, weapons, Equipment_Weapon, axe.Union(),
			path);

		fbb.Finish(orc);

		buf = fbb.GetBufferPointer();
		size = fbb.GetSize();
	}

	std::vector<uint8_t> vbuf; 

	vbuf.resize(size);
	std::memcpy(vbuf.data(), buf, size);

	// read
	{
		auto monster = GetMonster(vbuf.data());

		Verifier verifier(vbuf.data(), size);
		monster->Verify(verifier);

		// https://github.com/google/flatbuffers/pull/3905
		// - GetComputedSize() w/ FLATBUFFERS_TRACK_VERIFIER_BUFFER_SIZE

		auto cs = verifier.GetComputedSize();

		auto hp = monster->hp();
		auto mana = monster->mana();
		auto name = monster->name()->c_str();

		auto pos = monster->pos();
		auto x = pos->x();
		auto y = pos->y();
		auto z = pos->z();

		auto inv = monster->inventory(); // A pointer to a `flatbuffers::Vector<>`.
		auto inv_len = inv->size();
		auto third_item = inv->Get(2);

		auto weapons = monster->weapons(); // A pointer to a `flatbuffers::Vector<>`.
			
		auto weapon_len = weapons->size();
		auto second_weapon_name = weapons->Get(1)->name()->str();
		auto second_weapon_damage = weapons->Get(1)->damage();

		auto union_type = monster->equipped_type();
		if (union_type == Equipment_Weapon) {
			auto weapon = static_cast<const Weapon*>(monster->equipped()); // Requires `static_cast`
																		   // to type `const Weapon*`.
			auto weapon_name = weapon->name()->str(); // "Axe"
			auto weapon_damage = weapon->damage();    // 5
		}

		std::cout << "name: " << name << std::endl;
	}

}

void object_api()
{
	// flatc --cpp --gen_object_api monster.fbs

	MonsterT monster;

	monster.hp = 299;
	monster.path.push_back(Vec3(0, 1, 2));
	monster.path.push_back(Vec3(9, 0, 1));

	FlatBufferBuilder fbb;
	fbb.Finish(Monster::Pack(fbb, &monster));

	std::vector<uint8_t> vbuf;

	vbuf.resize(fbb.GetSize());
	std::memcpy(vbuf.data(), fbb.GetBufferPointer(), fbb.GetSize());

	MonsterT monster2;

	GetMonster(vbuf.data())->UnPackTo(&monster2);

	std::cout << "hp: " << monster2.hp << std::endl;
}

int main()
{
	tutorial();

	return 0;
}
