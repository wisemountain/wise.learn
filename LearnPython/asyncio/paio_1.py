# 

import asyncio 

async def foo(n):
    await asyncio.sleep(n)
    print(f"n: {n}")

async def main(): 
    tasks = [foo(1), foo(2), foo(3)]
    await asyncio.gather(*tasks)

asyncio.run(main())

# gather completes a task if the task completes


