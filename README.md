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
6. If PersistentCollector fails to persist the events, we don't retry again.
   Retry of older transactions happen with (and only with) arrival of new events.

## VSCode usage:
1. Run Task :
    `Ctrl-Shift-B` to build code.
    `Ctrl-P` -> press `>Run Task` -> Choose `private test` or `public test`.
2. Debugging test :
    Click on the `Debug Test` over the test name in VSCode.
    https://github.com/OmniSharp/omnisharp-vscode/wiki/How-to-run-and-debug-unit-tests

## Test:
```cmd
FOR /L %N IN () DO dotnet test --no-build --filter "FullyQualifiedName~OneAddAndSlideTaskConcurrently" --logger:"console;verbosity=detailed"
```

`Console.Write` comes in next line in test output. See `XunitConsoleWriter.cs`.

## Todo:
* Done : Have only one source in EventCollector.
* Done : Write few basic private test for each class.
* Done : Make multi threaded implementation test.
* Done : Pass multi threaded implementation.
* Pass ContinuationToken in the APIs.
* Pass previousLsn in TransactionApplied API.