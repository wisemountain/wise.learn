# 

import asyncio 

async def foo():
    await asyncio.sleep(5)
    print("Foo!")
    return 5

async def main(): 
    task = asyncio.create_task(foo())
    print(task)
    await asyncio.sleep(5)
    print("Hello World!")
    await asyncio.sleep(10)
    print(task)

asyncio.run(main())

# gather completes a task if the task completes


