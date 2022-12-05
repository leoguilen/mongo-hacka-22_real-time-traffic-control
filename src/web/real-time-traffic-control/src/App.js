/* eslint-disable jsx-a11y/anchor-is-valid */
import React, { useState, useEffect } from "react";
import * as Realm from "realm-web";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";
import Box from "@mui/material/Box";
import Card from "@mui/material/Card";
import CardHeader from "@mui/material/CardHeader";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import Stack from "@mui/material/Stack";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Visibility from "@mui/icons-material/Visibility";
import moment from "moment";

import "./App.css";

// Create the Application
const app = new Realm.App({ id: "traffic-control-dashboard-aszgc" });

// Define the App component
const App = () => {
  // Set state variables
  const [user, setUser] = useState();
  const [events, setEvents] = useState([]);
  const [open, setOpen] = React.useState(false);
  const [violation, setViolation] = useState({});

  const handleModalOpen = (vehiclePlate) => {
    user.app.currentUser
      .mongoClient("mongodb-atlas")
      .db("TRAFFIC_CONTROL")
      .collection("TRAFFIC_VIOLATIONS")
      .findOne({ "VEHICLE.LICENCE_PLATE": vehiclePlate })
      .then((violation) => {
        setViolation(violation);
        setOpen(true);
      })
      .catch((err) => console.error(err));
  };

  // This useEffect hook will run only once when the page is loaded
  useEffect(() => {
    const login = async () => {
      // Authenticate anonymously
      const user = await app.logIn(Realm.Credentials.anonymous());
      setUser(user);

      // Connect to the database
      const mongodb = app.currentUser.mongoClient("mongodb-atlas");
      const collection = mongodb
        .db("TRAFFIC_CONTROL")
        .collection("VEHICLE_DETECTED_EVENTS");

      // Everytime a change happens in the stream, add it to the list of events
      for await (const change of collection.watch()) {
        setEvents((events) => [...events, change.fullDocument]);
      }
    };
    login();
  }, []);

  return (
    <div className="App-Container">
      <nav className="App-Container-NavBar">
        <img
          height="50"
          alt="App Logo"
          src="https://th.bing.com/th/id/R.33486074ccf6b63a4430647280fd7f2c?rik=pJwHPnu2tXb0bw&riu=http%3a%2f%2fmytraffic.com.my%2fimages%2fTraffic-Controller-Icon-01.png&ehk=P98027%2bMA1R%2bTCf9945xNrTzrE%2bR9vEuX2qV5pJGEFk%3d&risl=&pid=ImgRaw&r=0"
        ></img>
        <h3>REAL TIME TRAFFIC CONTROL</h3>
      </nav>

      <div className="App-Container-Content">
        <Box
          sx={{
            display: "grid",
            maxHeight: "100%",
            gridTemplateColumns: "3fr 2fr",
            gap: 2,
            gridTemplateRows: "auto",
            gridTemplateAreas: `"table card"
                                "table ."`,
          }}
        >
          <Box
            sx={{
              gridArea: "table",
              height: "80vh",
              overflowY: "scroll",
            }}
          >
            <div className="App-Container-Card">
              <div className="App-Container-Card-Header">
                <img
                  height="30"
                  alt="Live icon"
                  src="https://cdn1.iconfinder.com/data/icons/youtube-23/29/Vector-10-512.png"
                ></img>
                <h4>Events</h4>
              </div>
              <div className="App-Container-Card-Content">
                <TableContainer component={Table} sx={{}}>
                  <TableHead>
                    <TableRow>
                      <TableCell>
                        <b>Timestamp</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>Lane</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>Vehicle Plate</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>Detected Speed</b>
                      </TableCell>
                      <TableCell align="right">
                        <b>Status</b>
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {events.map((e, i) => (
                      <TableRow
                        key={i}
                        sx={{
                          "&:last-child td, &:last-child th": { border: 0 },
                        }}
                      >
                        <TableCell component="th" scope="row">
                          {moment(e.TIMESTAMP).format("YYYY/MM/DD hh:mm:ss")}
                        </TableCell>
                        <TableCell align="right">{e.LANE_NUMBER}</TableCell>
                        <TableCell align="right">{e.VEHICLE_PLATE}</TableCell>
                        <TableCell align="right">
                          {e.DETECTED_SPEED} KM/h
                        </TableCell>
                        <TableCell align="right">
                          {Boolean(e.IS_SPEEDING) ? (
                            <Stack
                              direction="row"
                              justifyContent={"flex-end"}
                              alignItems={"center"}
                              spacing={1}
                            >
                              <Chip color="warning" label="Violation" />
                              <a
                                href="#"
                                onClick={() => handleModalOpen(e.VEHICLE_PLATE)}
                              >
                                <Visibility />
                              </a>
                            </Stack>
                          ) : (
                            <Chip color="info" label="Normal" />
                          )}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </TableContainer>
              </div>
            </div>
          </Box>
          <Box
            sx={{
              gridArea: "card",
              padding: 2,
              display: "flex",
              justifyContent: "space-around",
            }}
          >
            <Card sx={{ minWidth: 275 }}>
              <CardHeader title="Events Count:" />
              <CardContent
                sx={{
                  fontSize: 50,
                  textAlign: "center",
                  marginTop: "-15px",
                }}
              >
                {events.length}
              </CardContent>
            </Card>
            <Card sx={{ minWidth: 275 }}>
              <CardHeader title="Violations Count:" />
              <CardContent
                sx={{
                  fontSize: 50,
                  textAlign: "center",
                  marginTop: "-15px",
                  color: "red",
                }}
              >
                {events.filter((e) => Boolean(e.IS_SPEEDING)).length}
              </CardContent>
            </Card>
          </Box>
        </Box>
        <Dialog
          open={open}
          onClose={() => setViolation({})}
          aria-labelledby="dialog-title"
          aria-describedby="dialog-description"
        >
          <DialogTitle id="dialog-title">Violation Details</DialogTitle>
          <DialogContent>
            <DialogContentText id="dialog-description">
              {JSON.stringify(violation)}
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpen(false)}>Close</Button>
          </DialogActions>
        </Dialog>
      </div>
    </div>
  );
};

export default App;
