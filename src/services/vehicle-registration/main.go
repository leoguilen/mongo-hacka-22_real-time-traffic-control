package main

import (
	"os"

	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/logger"
)

func main() {
	app := fiber.New(fiber.Config{
		AppName:       "Vehicle Registration API",
		CaseSensitive: true,
		StrictRouting: true,
		GETOnly:       true,
	})
	repository := NewVehicleRegistrationRepositoryMock()

	app.Use(logger.New())

	app.Get("/api/vehicles/:licensePlate", func(c *fiber.Ctx) error {
		c.Accepts("application/json")

		licensePlate := c.Params("licensePlate")
		vehicle, err := repository.GetVehicle(licensePlate)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(fiber.Map{
				"error": err.Error(),
			})
		}

		return c.Status(fiber.StatusOK).JSON(vehicle)
	})

	panic(app.Listen(":" + os.Getenv("HTTP_PORT")))
}
