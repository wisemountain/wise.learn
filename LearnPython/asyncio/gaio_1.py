# https://medium.com/free-code-camp/a-guide-to-asynchronous-programming-in-python-with-asyncio-232e2afa44f6


import asyncio 
import time 
from datetime import datetime 

async def custom_sleep():
    print('SLEEP', datetime.now())
    await asyncio.sleep(1)

async def factorial(name, number):
    f = 1
    for i in range(2, number+1):
        print(f'Task {name} : Compute factorial({i})')
        await custom_sleep()
        f *= i

    print(f'Task {name}: factorial ({number}) is {f}')

start = time.time()

loop = asyncio.get_event_loop()

tasks = [
    asyncio.ensure_future(factorial('A', 10)),
    asyncio.ensure_future(factorial('B', 7))
]

loop.run_until_complete(asyncio.wait(tasks))
loop.close()

end = time.time()
print(f'Total time: {end - start}')