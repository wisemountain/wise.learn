# awaitable 
# - coroutine, task, future 

import asyncio 

async def nested(): 
    print("return value")
    return 42

async def main():
    task = asyncio.create_task(nested())
    await task

asyncio.run(main())