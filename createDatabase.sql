DROP DATABASE IF EXISTS ProyectoSO;

CREATE DATABASE ProyectoSO;
USE ProyectoSO;

CREATE TABLE Users (
	Player_ID INT(32) NOT NULL AUTO_INCREMENT,
	Username VARCHAR(32) NOT NULL,
	Password VARCHAR(32) NOT NULL,
	Profile_Picture VARCHAR(255),
	Profile_Notes VARCHAR(255),
	PRIMARY KEY(Player_ID)
);

CREATE TABLE Groups (
	Group_ID INT(32) NOT NULL AUTO_INCREMENT,
	Group_Name VARCHAR(32) NOT NULL,
	Last_User VARCHAR(32),
	Last_Message VARCHAR(255),
	Group_Notes VARCHAR(255),
	Profile_Picture VARCHAR(255),
	PRIMARY KEY(Group_ID)
);

CREATE TABLE Group_Members (
	Group_ID INT(32) NOT NULL,
	Player_ID INT(32) NOT NULL,
    	FOREIGN KEY(Group_ID) REFERENCES Groups(Group_ID),
    	FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)
);

CREATE TABLE Game (
	Game_ID INT(32) NOT NULL AUTO_INCREMENT,
	Group_ID INT(32) NOT NULL,
	Kind INT(8) NOT NULL,
	Status INT(32) NOT NULL,
	Time_Stamp VARCHAR(255) NOT NULL,
	PRIMARY KEY(Game_ID),
	FOREIGN KEY(Group_ID) REFERENCES Groups(Group_ID)
);


CREATE TABLE Game_Record (
	Game_ID INT(32) NOT NULL,
	Player_ID INT(32) NOT NULL,
	Points INT(8) NOT NULL,
	Variable_Field VARCHAR(255),
	FOREIGN KEY(Game_ID) REFERENCES Game(Game_ID),
	FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)

);

CREATE TABLE Chat (
	Message_ID INT(255) NOT NULL AUTO_INCREMENT,
	Group_ID INT(32) NOT NULL,
	Player_ID INT(32) NOT NULL,
	Message_Content VARCHAR(1024) NOT NULL,
	Time_Stampt VARCHAR(255) NOT NULL,
	PRIMARY KEY(Message_ID),
	FOREIGN KEY(Group_ID) REFERENCES Groups(Group_ID),
	FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)
);

CREATE TABLE Configuration (
	Version_ID INT(32) NOT NULL AUTO_INCREMENT,
	Mandatory INT(8) NOT NULL,
	Time_Stamp VARCHAR(255) NOT NULL,
	PRIMARY KEY(Version_ID)
);
