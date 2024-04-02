﻿using Editor.Component.Events;
using Editor.Component.Exceptions;

namespace Editor.Component;

public interface IEntity
{
    public bool Initialized { get; }
    public bool Alive { get; }
    public ComponentBase[] Components { get; init; }
    
    
    public void Init(IWorld world);
    public void Destroy();
    public ComponentRef<T>? GetComponent<T>() where T : ComponentBase;
    public ComponentRef<T> GetRequiredComponent<T>() where T : ComponentBase;
}

public sealed class Entity : IEntity
{
    public Entity()
    {
        Components = [];
    }

    internal Entity(IEnumerable<ComponentBase> components)
    {
        Components = components.ToArray();
    }
    
    
    public bool Alive { get; private set; }
    public bool Initialized { get; private set; }
    
    public ComponentBase[] Components { get; init; }


    public static IEntityBuilder CreateBuilder() => new EntityBuilder();
        
    public void Init(IWorld world)
    {
        if (Initialized)
        {
            return;
        }
        
        Initialized = true;
        Alive = true;
        
        foreach (var component in Components)
        {
            component.Init(world, this);
        }
    }

    public void Destroy()
    {
        foreach (var component in Components)
        {
            component.Destroy();
        }
        
        Initialized = false;
        Alive = false;
    }

    public ComponentRef<T>? GetComponent<T>() where T : ComponentBase
    {
        return Components.FirstOrDefault(x => x is T) is not T component 
            ? null 
            : new ComponentRef<T>(this, component);
    }

    public ComponentRef<T> GetRequiredComponent<T>() where T : ComponentBase
    {
        return GetComponent<T>() ?? throw new ComponentRequiredException(typeof(T));
    }
}