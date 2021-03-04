# coroutine

import asyncio 
import time

async def say_after(delay, what):
    await asyncio.sleep(delay)
    print(what)

async def main(): 
    task_1 = asyncio.create_task(say_after(1, 'hello'))
    task_2 = asyncio.create_task(say_after(2, 'world'))

    print(f"started at {time.strftime('%X')}")

    await task_1
    await task_2

    print(f"finished at {time.strftime('%X')}")

asyncio.run(main())