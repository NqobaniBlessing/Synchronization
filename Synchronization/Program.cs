using Synchronization.AsyncLock;

var taskLock = new AsyncLock();

var tasks = new List<Task>
{
    RunTaskAsync(1),
    RunTaskAsync(2),
    RunTaskAsync(3),
    RunTaskAsync(4)
};

await Task.WhenAll(tasks);
Console.WriteLine("All tasks complete.");
return;

async Task RunTaskAsync(int taskId)
{
    Console.WriteLine($"Task {taskId} waiting...");
    
    using (await taskLock.LockAsync())
    {
        Console.WriteLine($"Task {taskId} entered.");
        await Task.Delay(2000);
    }
}