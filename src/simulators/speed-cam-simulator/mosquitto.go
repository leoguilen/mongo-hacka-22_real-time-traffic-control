package main

import (
	"os"

	mqtt "github.com/eclipse/paho.mqtt.golang"
)

type Mosquitto struct {
	Client mqtt.Client
}

func NewMosquitto() *Mosquitto {
	return &Mosquitto{}
}

func (m *Mosquitto) Connect() {
	opts := mqtt.NewClientOptions()
	opts.AddBroker(os.Getenv("MQTT_BROKER"))
	opts.SetUsername(os.Getenv("MQTT_USERNAME"))
	opts.SetPassword(os.Getenv("MQTT_PASSWORD"))
	opts.SetClientID("speed-cam-simulator")
	m.Client = mqtt.NewClient(opts)
	if token := m.Client.Connect(); token.Wait() && token.Error() != nil {
		panic(token.Error())
	}
}
