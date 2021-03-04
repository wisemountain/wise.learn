from ursina import * 

def draw_grid(): 
    r = 8
    for i in range(1, r):
        t = i/r
        s = 4*i
        print(s)
        grid = Entity(model=Grid(s,s), scale=s, color=color.color(0,0,.8,lerp(.8,0,t)), rotation_x=90, position=(-s/2, i/1000, -s/2))
        subgrid = duplicate(grid)
        subgrid.model = Grid(s*4, s*4)
        subgrid.color = color.color(0,0,.4,lerp(.8,0,t))

app = Ursina()

EditorCamera()

s1 = Entity(model='sphere', scale=0.1, position=(0, 0, 0), color=(0, 0.7, 0.1, 1))

draw_grid()

app.run()

