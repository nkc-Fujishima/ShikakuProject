using System;

public interface IDestroy
{
    public event Action<IChaceable> OnDestroyHundle;
}