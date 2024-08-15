using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Post_Office_Management.Models
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.ServiceType)
                .WithMany()
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Delivery_ServiceType");

            builder.HasOne(d => d.FromOffice)
                .WithMany()
                .HasForeignKey(d => d.FromOfficeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Delivery_FromOffice");

            builder.HasOne(d => d.ToOffice)
                .WithMany()
                .HasForeignKey(d => d.ToOfficeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Delivery_ToOffice");
        }
    }
}