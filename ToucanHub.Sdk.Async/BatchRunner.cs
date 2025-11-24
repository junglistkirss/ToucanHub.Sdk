namespace ToucanHub.Sdk.Async;

public class BatchRunner<T>(Func<T, bool> canParallelize, Func<T, Task> getTask, int maxParallelTasks = 10)
{
    public async Task RunBatchAsync(IEnumerable<T> jobs, CancellationToken cancellationToken)
    {
        List<T> parallelJobs = [];
        List<T> serialJobs = [];

        foreach (T job in jobs)
        {
            if (canParallelize(job))
                parallelJobs.Add(job);
            else
                serialJobs.Add(job);
        }

        // Run parallel tasks with semaphore
        if (parallelJobs.Any())
        {
            using SemaphoreSlim semaphore = new(maxParallelTasks);
            List<Task> parallelize = [];
            foreach (T pJob in parallelJobs)
            {
                parallelize.Add(Task.Run(async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        await getTask(pJob);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }
            await Task.WhenAll(parallelize);
        }

        // Run serial tasks
        foreach (T sJob in serialJobs)
        {
            await getTask(sJob);
        }
    }
}