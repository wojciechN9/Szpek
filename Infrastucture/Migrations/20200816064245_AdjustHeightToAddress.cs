using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class AdjustHeightToAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
             //on server table has Address name - idk why there is such a diference
             @"
                    SET SQL_SAFE_UPDATES = 0;
                    UPDATE Address SET Height = 164 WHERE City = 'Michów' AND Street = 'Szkolna';
                    UPDATE Address SET Height = 200 WHERE City = 'Lublin' AND Street = 'Poligonowa';
                    UPDATE Address SET Height = 110 WHERE City = 'Warszawa' AND Street = 'Służby Polsce';
                    UPDATE Address SET Height = 245 WHERE City = 'Chmielnik' AND Street = 'Piastów';
                    UPDATE Address SET Height = 215 WHERE City = 'Lublin' AND Street = 'Godebskiego';
                    UPDATE Address SET Height = 133 WHERE City = 'Naumivka' AND Street = 'Sadova';
                    SET SQL_SAFE_UPDATES = 1;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
