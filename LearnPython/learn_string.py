# format

test = """
public static void Send{name}({arg})
{{
}}
"""

print(test.format(name='Bob', arg='int v, int[] vs'))


# f-string 

name='Move'
arg = 'int v, int[] vs'
rs = f"""
public static void Send{name}({arg})
{{
    // body comes here
}}
"""

print(rs)

# c++을 포함하여 f-string이 표준으로 되고 있다. 
# 편하고 성능도 괜찮다. 

# {{ }}로 중괄호를 쓰는 것도 c++과 같다. 


for c in name:
    print(c, "\n")
    print(c.upper(), "\n")