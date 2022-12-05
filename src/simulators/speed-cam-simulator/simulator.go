package main

import (
	"encoding/json"
	"fmt"
	"math/rand"
	"os"
	"strings"
	"time"

	"github.com/jaswdr/faker"
)

type Simulator struct {
	CamNumber int
	Mosquitto *Mosquitto
}

type Message struct {
	Timestamp     time.Time `json:"timestamp"`
	LaneNumber    int       `json:"lane_number"`
	VehiclePlate  string    `json:"vehicle_plate"`
	DetectedSpeed float32   `json:"detected_speed"`
}

func NewSimulator(camNumber int, mosquitto *Mosquitto) *Simulator {
	return &Simulator{
		CamNumber: camNumber,
		Mosquitto: mosquitto,
	}
}

func (s *Simulator) Run() {
	fmt.Printf("Running speed cam %d simulation\n", s.CamNumber)

	for {
		time.Sleep(time.Duration(s.CamNumber*(1000+rand.Intn(9999))) * time.Millisecond)
		go func() {
			message := Message{
				Timestamp:     time.Now(),
				LaneNumber:    s.CamNumber,
				VehiclePlate:  strings.ToUpper(faker.New().Bothify("???#?##")),
				DetectedSpeed: float32(50 + rand.Intn(150)),
			}

			fmt.Printf("%s - Speed cam %d detected a car with plate %s at %d km/h\n", time.Now().Format(time.RFC3339), s.CamNumber, message.VehiclePlate, int(message.DetectedSpeed))

			json, err := json.Marshal(message)
			if err != nil {
				fmt.Println(err)
			}
			token := s.Mosquitto.Client.Publish(os.Getenv("MQTT_TOPIC"), 0, false, json)
			token.Wait()
		}()
	}
}
