package main

import (
	"math/rand"
	"os"
	"strconv"
)

func check(e error) {
	if e != nil {
		panic(e)
	}
}

func main() {
	filenamePrefix := "problem"
	for i := 0; i < 30; i++ {
		str2 := filenamePrefix + strconv.Itoa(i) + ".txt"
		f, err := os.Create(str2)
		check(err)
		defer f.Close()

		numberOfJobs := i*10 + 10
		f.WriteString(strconv.Itoa(numberOfJobs) + "\n")
		for j := 0; j < numberOfJobs; j++ {
			r := rand.Intn(5000) + 100
			p := rand.Intn(100) + 10
			q := rand.Intn(5000) + 100
			line := strconv.Itoa(r) + " " + strconv.Itoa(p) + " " + strconv.Itoa(q)
			f.WriteString(line + "\n")
		}

	}
}
