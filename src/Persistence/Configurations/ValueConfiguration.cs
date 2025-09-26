namespace Persistence.Configurations;

public class ValueConfiguration : IEntityTypeConfiguration<Value>
{
    public void Configure(EntityTypeBuilder<Value> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.ValueNumber).IsRequired();
        builder.Property(e => e.StatusId).IsRequired();
    }
}
