//namespace Toucan.Sdk.Utils;

//public static class TaskExtensions
//{
//    public static Task Then(this Task task, Action<Task> onComplete, Action<Task> onFail)
//      => task.ContinueWith(prev =>
//      {
//          switch (prev.Status)
//          {
//              case TaskStatus.RanToCompletion:
//                  onComplete(prev);
//                  break;
//              case TaskStatus.Canceled:
//              case TaskStatus.Faulted:
//                  onFail(prev);
//                  break;
//          }
//      });

//    public static Task Then<T>(this Task<T> task, Action<Task<T>> onComplete, Action<Task<T>> onFail)
//      => task.ContinueWith(prev =>
//      {
//          switch (prev.Status)
//          {
//              case TaskStatus.RanToCompletion:
//                  onComplete(prev);
//                  break;
//              case TaskStatus.Canceled:
//              case TaskStatus.Faulted:
//                  onFail(prev);
//                  break;
//          }
//      });

//    public static Task<TOut> Then<T, TOut>(this Task<T> task, Func<Task<T>, TOut> onComplete, Func<Task<T>, TOut> onFail, Func<TOut> defaultOut)
//      => task.ContinueWith(prev =>
//      {
//          switch (prev.Status)
//          {
//              case TaskStatus.RanToCompletion:
//                  return onComplete(prev);
//              case TaskStatus.Canceled:
//              case TaskStatus.Faulted:
//                  return onFail(prev);
//          }
//          return defaultOut();
//      });

//    public static Task OnComplete(this Task task, Action<Task> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnRanToCompletion);

//    public static Task OnComplete<T>(this Task<T> task, Action<Task<T>> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnRanToCompletion);

//    public static Task<TOut> OnComplete<T, TOut>(this Task<T> task, Func<Task<T>, Task<TOut>> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();

//    public static Task<TOut> OnComplete<T, TOut>(this Task<T> task, Func<T, Task<TOut>> action)
//        => task.ContinueWith(prev => action(prev.Result), TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap();

//    public static Task<TOut> OnComplete<T, TOut>(this Task<T> task, Func<Task<T>, TOut> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnRanToCompletion);

//    public static Task<TOut> OnComplete<T, TOut>(this Task<T> task, Func<T, TOut> action)
//        => task.ContinueWith(prev => action(prev.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
//    public static Task OnFailure(this Task task, Action<Task> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnFaulted);

//    public static Task OnFailure<T>(this Task<T> task, Action<Task<T>> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnFaulted);

//    public static Task<TOut> OnFailure<T, TOut>(this Task<T> task, Func<Task<T>, Task<TOut>> action)
//       => task.ContinueWith(action, TaskContinuationOptions.OnlyOnFaulted).Unwrap();

//    public static Task<TOut> OnFailure<T, TOut>(this Task<T> task, Func<T, Task<TOut>> action)
//       => task.ContinueWith(prev => action(prev.Result), TaskContinuationOptions.OnlyOnFaulted).Unwrap();

//    public static Task<TOut> OnFailure<T, TOut>(this Task<T> task, Func<Task<T>, TOut> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnFaulted);


//    public static Task<TOut> OnFailure<T, TOut>(this Task<T> task, Func<T, TOut> action)
//        => task.ContinueWith(prev => action(prev.Result), TaskContinuationOptions.OnlyOnFaulted);

//    public static Task OnCancel(this Task task, Action<Task> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnCanceled);

//    public static Task OnCancel<T>(this Task task, Action<Task> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnCanceled);

//    public static Task<TOut> OnCancel<T, TOut>(this Task<T> task, Func<Task<T>, TOut> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnCanceled);
//    public static Task<TOut> OnCancel<T, TOut>(this Task<T> task, Func<T, TOut> action)
//        => task.ContinueWith(prev => action(prev.Result), TaskContinuationOptions.OnlyOnCanceled);
//    public static Task<TOut> OnCancel<T, TOut>(this Task<T> task, Func<Task<T>, Task<TOut>> action)
//        => task.ContinueWith(action, TaskContinuationOptions.OnlyOnCanceled).Unwrap();

//    public static Task<TOut> OnCancel<T, TOut>(this Task<T> task, Func<T, Task<TOut>> action)
//        => task.ContinueWith(prev => action(prev.Result), TaskContinuationOptions.OnlyOnCanceled).Unwrap();
//}

