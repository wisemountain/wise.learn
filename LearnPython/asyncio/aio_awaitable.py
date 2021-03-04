# awaitable 
# - coroutine, task, future 

import asyncio 

async def nested(): 
    return 42

async def main():
    # not executed.
    nested()

    print(await nested())

asyncio.run(main())