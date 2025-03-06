import asyncio
from websockets.asyncio.server import serve


async def handler(websocket):
    try:
        await websocket.send(bytes([10, 84, 101, 115, 116, 32, 82, 101, 109, 111, 116, 101, 0, 0, 0, 0, 0, 0, 0, 0, 0])) # Handshake

        async for message in websocket:
            if (message == b'\xfa'):
                await websocket.send(bytes([255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0])) # Simulates guide button
    finally:
        pass

async def main():
    async with serve(handler, "192.168.100.8", 26401) as server:
        await server.serve_forever()

if __name__ == "__main__":
    asyncio.run(main())