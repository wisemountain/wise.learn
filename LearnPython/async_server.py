import asyncio

async def handle_conn(reader, writer):
  # 입출력 핸들러는 항상 StreamReader, StreamWriter를 인자로 받는다.
  bufsize = 1024
  addr = writer.get_extra_info('peername')
  while True:
    data = await reader.read(bufsize)
    msg = data.decode()
    print("RECEIVED {} FROM {}".format(msg, addr))
    print("SENDING {}".format(msg))
    writer.write(data)
    await writer.drain()
    if msg == 'bye':
      print("CLOSING")
      writer.close()
      await writer.wait_closed()
      break

async def run_server(host='127.0.0.1', port=7000):
  server = await asyncio.start_server(handle_conn, host, port)
  addr = server.sockets[0].getsockname()
  print("SERVING ON {}".format(addr))

  async with server:
    await server.serve_forever()

asyncio.get_event_loop().run_until_complete(run_server())


#
# asyncio 내부적으로 overlapped를 windows 에서 사용하는 등 
# 각 플래폼에서 빠른 구현을 갖추고 있다. 
# 이와 같은 구조를 배우고 익혀서 봇들에 적용한다. 
# C#으로 개발하는 스크립트에서 python을 배우고 익혀서 좋은 봇툴을 만든다. 
# 

#
# async / await는 c#과 비슷하지만 task 대신 coroutine이라는 점이 다르다. 
# 