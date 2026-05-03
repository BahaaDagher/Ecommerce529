using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce529.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInBrandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (name, description, logo, status) values ('Apple', 'Maecenas tincidunt lacus at velit.', 'Oppo.png', 1);insert into Brands (name, description, logo, status) values ('Samsung', 'Aliquam sit amet diam in magna bibendum imperdiet.', 'Vivo.png', 1);insert into Brands (name, description, logo, status) values ('Oppo', 'Nulla suscipit ligula in lacus.', 'Vivo.png', 1);insert into Brands (name, description, logo, status) values ('Sony', 'Donec dapibus.', 'Sony.png', 1);insert into Brands (name, description, logo, status) values ('Hp', 'In blandit ultrices enim.', 'Apple.png', 0);insert into Brands (name, description, logo, status) values ('Lenovo', 'Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci.', 'Sony.png', 1);insert into Brands (name, description, logo, status) values ('Realme', 'Sed vel enim sit amet nunc viverra dapibus.', 'Sony.png', 1);insert into Brands (name, description, logo, status) values ('Vivo', 'Aliquam non mauris.', 'Sony.png', 0);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Brands");
        }
    }
}
