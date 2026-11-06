[![progress-banner](https://backend.codecrafters.io/progress/dns-server/035c1e1b-a0df-4f0b-8d98-90f5ebc0b12a)](https://app.codecrafters.io/users/KiwiGeek?r=2qF)

This is a starting point for C# solutions to the
["Build Your Own DNS server" Challenge](https://app.codecrafters.io/courses/dns-server/overview).

In this challenge, you'll build a DNS server that's capable of parsing and
creating DNS packets, responding to DNS queries, handling various record types
and doing recursive resolve. Along the way we'll learn about the DNS protocol,
DNS packet format, root servers, authoritative servers, forwarding servers,
various record types (A, AAAA, CNAME, etc) and more.

**Note**: If you're viewing this repo on GitHub, head over to
[codecrafters.io](https://codecrafters.io) to try the challenge.

# Passing the first stage

The entry point for your `your_program.sh` implementation is in `src/Server.cs`.
Study and uncomment the relevant code, and then run the command below to execute
the tests on our servers:

```sh
codecrafters submit
```

Time to move on to the next stage!

# Stage 2 & beyond

Note: This section is for stages 2 and beyond.

1. Ensure you have `dotnet (9.0)` installed locally
1. Run `./your_program.sh` to run your program, which is implemented in
   `src/Server.cs`.
1. Run `codecrafters submit` to submit your solution to CodeCrafters. Test
   output will be streamed to your terminal.
