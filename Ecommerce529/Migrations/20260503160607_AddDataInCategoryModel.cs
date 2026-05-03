using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce529.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (name, description, status) values ('Mobiles', 'Nulla ac enim.', 1);insert into Categories (name, description, status) values ('Tablets', 'Integer ac neque.', 1);insert into Categories (name, description, status) values ('PCs', 'Cras pellentesque volutpat dui.', 0);insert into Categories (name, description, status) values ('Laptops', 'Donec semper sapien a libero.', 1);insert into Categories (name, description, status) values ('Cameras', 'Aliquam quis turpis eget elit sodales scelerisque.', 1);insert into Categories (name, description, status) values ('Accessories', 'Morbi a ipsum.', 1);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Categories"); 
        }
    }
}
