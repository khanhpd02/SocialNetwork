using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Entity;

public partial class SocialNetworkContext : DbContext
{
    public SocialNetworkContext()
    {
    }

    public SocialNetworkContext(DbContextOptions<SocialNetworkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdministrativeRegion> AdministrativeRegions { get; set; }

    public virtual DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupChat> GroupChats { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Infor> Infors { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<MasterDatum> MasterData { get; set; }

    public virtual DbSet<Notify> Notifies { get; set; }

    public virtual DbSet<PinCode> PinCodes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Share> Shares { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagPost> TagPosts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGroupChat> UserGroupChats { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016;Initial Catalog=truongnetwwork_SocialNetwork;uid=truongnetwwork_SocialNetwork;pwd=social;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdministrativeRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("administrative_regions_pkey");

            entity.ToTable("administrative_regions");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CodeName)
                .HasMaxLength(255)
                .HasColumnName("code_name");
            entity.Property(e => e.CodeNameEn)
                .HasMaxLength(255)
                .HasColumnName("code_name_en");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name_en");
        });

        modelBuilder.Entity<AdministrativeUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("administrative_units_pkey");

            entity.ToTable("administrative_units");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CodeName)
                .HasMaxLength(255)
                .HasColumnName("code_name");
            entity.Property(e => e.CodeNameEn)
                .HasMaxLength(255)
                .HasColumnName("code_name_en");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.FullNameEn)
                .HasMaxLength(255)
                .HasColumnName("full_name_en");
            entity.Property(e => e.ShortName)
                .HasMaxLength(255)
                .HasColumnName("short_name");
            entity.Property(e => e.ShortNameEn)
                .HasMaxLength(255)
                .HasColumnName("short_name_en");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Chat_1");

            entity.ToTable("Chat");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC075B05C99E");

            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Comment_Comment");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("districts_pkey");

            entity.ToTable("districts");

            entity.HasIndex(e => e.ProvinceCode, "idx_districts_province");

            entity.HasIndex(e => e.AdministrativeUnitId, "idx_districts_unit");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");
            entity.Property(e => e.CodeName)
                .HasMaxLength(255)
                .HasColumnName("code_name");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.FullNameEn)
                .HasMaxLength(255)
                .HasColumnName("full_name_en");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");
            entity.Property(e => e.ProvinceCode)
                .HasMaxLength(20)
                .HasColumnName("province_code");

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.Districts)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .HasConstraintName("districts_administrative_unit_id_fkey");

            entity.HasOne(d => d.ProvinceCodeNavigation).WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceCode)
                .HasConstraintName("districts_province_code_fkey");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friends__3214EC0754E9F42D");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.UserAcceptNavigation).WithMany(p => p.FriendUserAcceptNavigations)
                .HasForeignKey(d => d.UserAccept)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friends_User1");

            entity.HasOne(d => d.UserToNavigation).WithMany(p => p.FriendUserToNavigations)
                .HasForeignKey(d => d.UserTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friends_User");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Group__3214EC07A6F61641");

            entity.ToTable("Group");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GroupCha__3214EC07DF63178C");

            entity.ToTable("GroupChat");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC07A4C02BB6");

            entity.ToTable("Image");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Infor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Infor__3214EC07F6CEB489");

            entity.ToTable("Infor");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Infors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Infor_User");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Like__3214EC07C89EF59F");

            entity.ToTable("Like");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Like_User");
        });

        modelBuilder.Entity<MasterDatum>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notify>(entity =>
        {
            entity.ToTable("Notify");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.NotifyType).HasColumnName("notifyType");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PinCode>(entity =>
        {
            entity.ToTable("PinCode");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredTime).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC07CB1999EA");

            entity.ToTable("Post");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Post_User");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("provinces_pkey");

            entity.ToTable("provinces");

            entity.HasIndex(e => e.AdministrativeRegionId, "idx_provinces_region");

            entity.HasIndex(e => e.AdministrativeUnitId, "idx_provinces_unit");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.AdministrativeRegionId).HasColumnName("administrative_region_id");
            entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");
            entity.Property(e => e.CodeName)
                .HasMaxLength(255)
                .HasColumnName("code_name");
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.FullNameEn)
                .HasMaxLength(255)
                .HasColumnName("full_name_en");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");

            entity.HasOne(d => d.AdministrativeRegion).WithMany(p => p.Provinces)
                .HasForeignKey(d => d.AdministrativeRegionId)
                .HasConstraintName("provinces_administrative_region_id_fkey");

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.Provinces)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .HasConstraintName("provinces_administrative_unit_id_fkey");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RefreshToken");

            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Report__3214EC07FB506DE0");

            entity.ToTable("Report");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Reports)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Report_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Report_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC074D6E6D35");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Share>(entity =>
        {
            entity.ToTable("Share");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Shares)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Share_User");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tag__3214EC07E45D22A9");

            entity.ToTable("Tag");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TagPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tag_Post_1");

            entity.ToTable("Tag_Post");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.TagPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Tag_Post_Post");

            entity.HasOne(d => d.Tag).WithMany(p => p.TagPosts)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK_Tag_Post_Tag");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07FA4B2288");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserGroupChat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_Group");

            entity.ToTable("User_GroupChat");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.GroupChat).WithMany(p => p.UserGroupChats)
                .HasForeignKey(d => d.GroupChatId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_GroupChat_GroupChat");

            entity.HasOne(d => d.User).WithMany(p => p.UserGroupChats)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_GroupChat_User");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_Role_1");

            entity.ToTable("User_Role");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User_Role_User");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Video__3214EC074B0367F0");

            entity.ToTable("Video");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Videos)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Video_Post");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("wards_pkey");

            entity.ToTable("wards");

            entity.HasIndex(e => e.DistrictCode, "idx_wards_district");

            entity.HasIndex(e => e.AdministrativeUnitId, "idx_wards_unit");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");
            entity.Property(e => e.CodeName)
                .HasMaxLength(255)
                .HasColumnName("code_name");
            entity.Property(e => e.DistrictCode)
                .HasMaxLength(20)
                .HasColumnName("district_code");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.FullNameEn)
                .HasMaxLength(255)
                .HasColumnName("full_name_en");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(255)
                .HasColumnName("name_en");

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.Wards)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .HasConstraintName("wards_administrative_unit_id_fkey");

            entity.HasOne(d => d.DistrictCodeNavigation).WithMany(p => p.Wards)
                .HasForeignKey(d => d.DistrictCode)
                .HasConstraintName("wards_district_code_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
