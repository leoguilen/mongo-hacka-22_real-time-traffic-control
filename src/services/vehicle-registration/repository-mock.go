package main

import (
	"github.com/google/uuid"
	"github.com/jaswdr/faker"
)

type Vehicle struct {
	Id           uuid.UUID `json:"id"`
	LicensePlate string    `json:"licensePlate"`
	Make         string    `json:"make"`
	Model        string    `json:"model"`
	Color        string    `json:"color"`
	OwnerName    string    `json:"ownerName"`
	OwnerEmail   string    `json:"ownerEmail"`
}

var _faker = faker.New()

type VehicleRegistrationRepository interface {
	GetVehicle(licencePlate string) (*Vehicle, error)
}

type VehicleRegistrationRepositoryMock struct {
}

func NewVehicleRegistrationRepositoryMock() VehicleRegistrationRepository {
	return &VehicleRegistrationRepositoryMock{}
}

// GetVehicle implements VehicleRegistrationRepository
func (*VehicleRegistrationRepositoryMock) GetVehicle(licencePlate string) (*Vehicle, error) {
	var vehicle = Vehicle{
		Id:           uuid.New(),
		LicensePlate: licencePlate,
		Make:         _faker.Car().Maker(),
		Model:        _faker.Car().Model(),
		Color:        _faker.Color().ColorName(),
		OwnerName:    _faker.Person().Name(),
		OwnerEmail:   _faker.Person().Contact().Email,
	}

	return &vehicle, nil
}
