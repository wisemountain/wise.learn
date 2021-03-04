from lark import Lark, Transformer, Tree, Token

gf = open("flatbuf.lark", "r")
glark = gf.read()
gf.close()

parser = Lark(glark)

lf = open("monster.fbs", "r")
lang = lf.read()
lf.close()

monster_tree = parser.parse(lang)

class MonsterTree(Transformer):

    def include_decl(self, s):
        for c in s:
            print("include => ", c)

    def namespace_decl(self, s):
        pass

    def type_decl(self, s): 
        pass

    def type_decl_name(self, s):
        print("typename: ", s[0], "\n")

    def field_decl(self, s):
        pass

    def field_name(self, s):
        print("fieldname: ", s[0], "\n")


monster_transform = MonsterTree() 
monster_transform.transform(monster_tree)