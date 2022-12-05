package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"
)

func main() {
	sigChannel := make(chan os.Signal, 1)
	signal.Notify(sigChannel, os.Interrupt, syscall.SIGTERM)

	lanes := 3
	simulators := make([]*Simulator, lanes)
	mosquitto := NewMosquitto()
	mosquitto.Connect()

	for i := 0; i < lanes; i++ {
		simulators[i] = NewSimulator(i+1, mosquitto)
	}

	for _, simulator := range simulators {
		go simulator.Run()
	}

	fmt.Println("Try pressing CTRL + C to exit")

	<-sigChannel

	fmt.Println("Exiting...")
	os.Exit(1)
}
