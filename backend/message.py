# import os, time

# p_path = "message_pipe"

# if not os.path.exists(p_path):
#     os.mkfifo(p_path)

# p_fd = os.open(p_path, os.O_RDONLY | os.O_NONBLOCK)

# with os.fdopen(p_fd) as pipe:
#     while True:
#         message = pipe.read()

#         if message:
#             print("> %s" % message)
#         time.sleep(0.1)
# print('Run classification...')

import time
import sys

while True:
    input = sys.stdin.readline()
    print(input)
    time.sleep(0.1)