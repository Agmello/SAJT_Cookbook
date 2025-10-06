using System;

namespace SAJT.Cookbook.Domain.Entities;

public sealed class User
{
    private User()
    {
    }

    private User(Guid id, string name)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name cannot be empty.", nameof(name))
            : name.Trim();
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public static User Create(string name)
    {
        return new User(Guid.NewGuid(), name);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

