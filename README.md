# Event Collector

Build and run tests before pushing
```
powershell ./build_tests.ps1
```

## Design
1. Event collector collect events and store in temporary queue.
2. Producers push events to it.
3. It pushes events out to target plugin.
4. It retries on transient failures.
5. It uses Ihealthstore to emit warnings / errors.

Think about APIs :
1. TransactionApplied<T>(LSN, T change)
    Serialize the change and send over wire.
2. TransactionApplied(LSN, byte [] change)
    Serialize yourself.

## VSCode usage:
1. Run Task :
    `Ctrl-Shift-B` to build code.
    `Ctrl-P` -> press `>Run Task` -> Choose `private test` or `public test`.
2. Debugging test :
    Click on the `Debug Test` over the test name in VSCode.
    https://github.com/OmniSharp/omnisharp-vscode/wiki/How-to-run-and-debug-unit-tests
