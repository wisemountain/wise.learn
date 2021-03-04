from lark import Lark

gf = open("flat_re.lark", "r")
glark = gf.read()
gf.close()

parser = Lark(glark)

lang = """
    namespace test.hello;

    table t1 {
        f1 : string;
        f2 : int;
    }

    enum item_category {
        v1, 
        c2, 
        c3
    }
"""
print(parser.parse(lang).pretty())
