#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#include <time.h>

// DECLARATION OF VARIABLES:
int i;
int sockets[100];
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
int dice1;
int dice2;

// DECLARATION OF STRUCTURES FOR THE LIST OF CLIENTS
typedef struct {
	char nombre [20];
	int socket;
} Conectado;

typedef struct {
	Conectado conectados [100];
	int num;
} ListaConectados;

ListaConectados miLista;

// DECLARATION OF FUNCTIONS FOR THE LIST OF CLIENTS
int Pon (ListaConectados *lista, char nombre[20], int socket) {
// |-----------------------------------------------------------------------------------------------------------|
// | Function: Pon                                                                                             |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Add a new client to the connected list.                                                      |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - ListaConectados *lista: Pointer to the list of connected clients.                                      |
// |  - char nombre[20]: The name of the client to be added.                                                   |
// |  - int socket: The socket descriptor associated with the client.                                          |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// |  - Returns 0 if the client was added successfully.                                                        |
// |  - Returns -1 if the list was already full and the client could not be added.                             |
// |-----------------------------------------------------------------------------------------------------------|
	if (lista->num == 100)
		return -1;
	else {
		strcpy(lista->conectados[lista->num].nombre, nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num++;
		return 0;
	}
}

int DamePosicion (ListaConectados *lista, char nombre[20]) {
// |-----------------------------------------------------------------------------------------------------------|
// | Function: DamePosicion                                                                                    |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Find the position of a client in the connected list by their name.                           |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - ListaConectados *lista: Pointer to the list of connected clients.                                      |
// |  - char nombre[20]: The name of the client whose position is being searched for.                          |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// |  - Returns the position of the client in the list if found.                                               |
// |  - Returns -1 if the client is not found in the list.                                                     |
// |-----------------------------------------------------------------------------------------------------------|
	int i = 0;
	int encontrado = 0;
	while ((i < lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
			encontrado = 1;
		if (!encontrado)
			i = i + 1;
	}
	if (encontrado)
		return i;
	else
		return -1;
}

int Elimina (ListaConectados *lista, char nombre[20]) {
// |-----------------------------------------------------------------------------------------------------------|
// | Function: Elimina                                                                                         |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Removes a client from the connected list by their name.                                      |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - ListaConectados *lista: Pointer to the list of connected clients.                                      |
// |  - char nombre[20]: The name of the client to be removed.                                                 |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// |  - Returns 0 if the client was successfully removed from the list.                                        |
// |  - Returns -1 if the client is not found in the list.                                                     |
// |-----------------------------------------------------------------------------------------------------------|
	int pos = DamePosicion (lista, nombre);
	if (pos == -1)
		return -1;
	else {
		int i;
		for (i=pos; i < lista->num-1; i++)
		{
			lista->conectados[i] = lista->conectados[i+1];
			// strcpy (lista->conectados[i].nombre, lista->conectados[i+1].nombre);
			// lista->conectados[i].socket = lista->conectados[i+1].socket;
		}
		lista->num--;
		return 0;
	}
}

void DameConectados(ListaConectados *lista, char conectados[300], char user[300]) {
// |-----------------------------------------------------------------------------------------------------------|
// | Function: DameConectados                                                                                  |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Retrieves the list of connected clients (excluding the current user).                        |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - ListaConectados *lista: Pointer to the list of connected clients.                                      |
// |  - char conectados[300]: String buffer where the result will be stored.  								   |
// |  - const char *username: The username of the client executing the command.								   |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// |  - The string will contain the client names, excluding the current user's username.	    	    	   |
// |-----------------------------------------------------------------------------------------------------------|
	conectados[0] = '\0';
	for (int i = 0; i < lista->num; i++) {
		if (strcmp(lista->conectados[i].nombre, user) != 0) {
			if (conectados[0] != '\0') {
				strcat(conectados, "\n");
			}
		strcat(conectados, lista->conectados[i].nombre);
		}
	}
}

// DECLARATION OF FUNCTIONS FOR THE CHAT
void EnviarATodos(ListaConectados *lista, char *mensaje, int emisor_socket) {
// |-----------------------------------------------------------------------------------------------------------|
// | Function: Send message to all connected clients		                                                   |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: Iterates through the list of connected clients and sends a specified message to all clients. |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - ListaConectados *lista: Pointer to the list of connected clients.                                      |
// |  - char *mensaje: The message to be sent to the clients.                                                  |
// |  - int emisor_socket: The socket descriptor of the client that sent the original message.                 |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:		                                                                                           |
// |  - Sends the message to all clients.			                                                           |
// |-----------------------------------------------------------------------------------------------------------|
/*	pthread_mutex_lock(&mutex);*/
/*	for (int i = 0; i < lista->num; i++) {*/
/*		if (lista->conectados[i].socket != emisor_socket) {*/
/*			write(lista->conectados[i].socket, mensaje, strlen(mensaje));*/
/*		}*/
/*	}*/
/*	pthread_mutex_unlock(&mutex);*/
	
	pthread_mutex_lock(&mutex);
	for (int i = 0; i < lista->num; i++) {
		if (lista->conectados[i].socket != emisor_socket) {
			// Manejar el error en el write
			if (write(lista->conectados[i].socket, mensaje, strlen(mensaje)) < 0) {
				perror("Error enviando mensaje");
				close(lista->conectados[i].socket);
			}
		}
	}
	pthread_mutex_unlock(&mutex);
}

/*void EnviarPrivado(ListaConectados *lista, char *receptor, char *mensaje) {*/
// |-----------------------------------------------------------------------------------------------------------|
// | Function: Send a private message to a specific connected client                                           |
// |-----------------------------------------------------------------------------------------------------------|
// | Description: This function sends a private message to a specified recipient if they are connected.        |
// |-----------------------------------------------------------------------------------------------------------|
// | Input:                                                                                                    |
// |  - ListaConectados *lista: Pointer to the list of connected clients.                                      |
// |  - char *receptor: The name of the recipient client to whom the message is being sent.                    |
// |  - char *mensaje: The message to be sent to the recipient.                                                |
// |-----------------------------------------------------------------------------------------------------------|
// | Output:                                                                                                   |
// |  - Sends the message to the specified recipient if they are found in the connected clients list.          |
// |-----------------------------------------------------------------------------------------------------------|
/*	pthread_mutex_lock(&mutex);*/
/*	int pos = DamePosicion(lista, receptor);*/
/*	if (pos != -1) {*/
/*		write(lista->conectados[pos].socket, mensaje, strlen(mensaje));*/
/*	}*/
/*	pthread_mutex_unlock(&mutex);*/
/*}*/


// DECLARATION OF PARAMETERS FOR THE CONCURRENT SERVER
void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	// int socket_conn = * (int *) socket;
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	MYSQL *conn;
	int err;
	
	// Create a connection to the MYSQL server
	conn = mysql_init(NULL);
	if (conn == NULL) {
		printf ("Error creating connection: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	// Initialize the connection, entering our access keys and the database name
	conn = mysql_real_connect (conn, "localhost", "root", "mysql", "ProyectoSO", 0, NULL, 0);
	if (conn == NULL) {
		printf ("Error initializing the connection: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	int terminar =0;
	// We enter a loop to handle all requests from this client until they disconnect
	while (terminar ==0)
	{
		// We receive the request
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// We need to add the null terminator to prevent it from writing beyond the buffer
		peticion[ret]='\0';
		
		printf("Peticion: %s\n", peticion);
		
		// Request
		char *p = strtok(peticion, "/");
		int codigo = atoi (p);
		
		char nombre[25];
		char password[25];
		char username[300];
		
		if (codigo !=0)
		{
			p = strtok( NULL, "/");
			if (p != NULL) {
				strcpy(nombre, p);
				printf("Código: %d, Nombre: %s\n", codigo, nombre);
				if (codigo == 2) {
					strcpy(username, nombre);
				}
					
			} 
			else {
				printf("Código: %d, sin nombre adicional.\n", codigo);
				nombre[0] = '\0';
			}
		}

		if (codigo == 0) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Close connection + Delete a client from the list of clients                                        |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Handles the request to disconnect a client and remove them from the list.                    |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 0/                                                                                                 |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sets 'terminar' to 1 to indicate the process should terminate.                                         |
		// |  - Calls the Elimina function to remove the specified client from the list.                               |
		// |-----------------------------------------------------------------------------------------------------------|
			terminar = 1;
			Elimina(&miLista, nombre);
		} 
		
		else if (codigo == 1) { 
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Register	                                                                                       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Handles user registration.			                            					       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 1/Username/Password                                                                                |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sets 'respuesta' to 0 if the username already exists.                                                  |
		// |  - Sets 'respuesta' to 1 if the user is registered successfully.                                          |
		// |-----------------------------------------------------------------------------------------------------------|
			p = strtok(NULL, "/");
			strcpy(password, p);
			printf("Password: %s\n", password);
			
			// Check if the user already exists
			char query[256];
			sprintf(query, "SELECT Username FROM Users WHERE Username='%s'", nombre);
			err = mysql_query(conn, query);
			
			if (err != 0) {
				printf ("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			MYSQL_RES *result = mysql_store_result(conn);
			int num_rows = mysql_num_rows(result);
			mysql_free_result(result);
			
			if (num_rows > 0) {
				sprintf(respuesta, "0");  // Username already exists
			} else {
				// Insert the new user into the database
				sprintf(query, "INSERT INTO Users (Username, Password) VALUES ('%s', '%s')", nombre, password);
				err = mysql_query(conn, query);
				
				if (err != 0) {
					printf ("Error inserting data into database: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				} else {
					sprintf(respuesta, "1");  // User registered successfully
					
				}
			}
		} 
		
		else if (codigo == 2) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Log In + Insert a client in the list of clients                                                    |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Handles user login by checking if the username exists and if the provided password matches.  |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 2/Username/Password                                                                                |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sets 'respuesta' to 0 if the login fails (username or password does not match).                        |
		// |  - Sets 'respuesta' to 1 if the login is successful.                                                      |
		// |  - Calls the Pon function to add the user to the list of connected users if login is successful.          |
		// |-----------------------------------------------------------------------------------------------------------|
			p = strtok(NULL, "/");
			strcpy(password, p);
			
			// Check if the user exists and password matches
			char query[256];
			sprintf(query, "SELECT Username, Password FROM Users WHERE Username='%s' AND Password='%s'", nombre, password);
			err = mysql_query(conn, query);
			
			if (err != 0) {
				printf ("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			MYSQL_RES *result = mysql_store_result(conn);
			int num_rows = mysql_num_rows(result);
			mysql_free_result(result);
			
			if (num_rows == 0) {
				sprintf(respuesta, "0");  // Login failed
			} else {
				sprintf(respuesta, "1");  // Login successful
				Pon(&miLista, nombre, sock_conn);
			}
		}
		
		else if (codigo == 3) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Select players of a game	                                                                       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Retrieves the usernames of players participating in a specific game.                         |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 3/Game_ID                                                                                          |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Includes in 'respuesta' the usernames of players in the specified game.       	                       |
		// |-----------------------------------------------------------------------------------------------------------|
			int game_id = atoi(p);
			
			// Prepare the query
			char select_query[256];
			snprintf(select_query, sizeof(select_query), "SELECT DISTINCT Users.Username FROM Users JOIN Game_Record ON Users.Player_ID = Game_Record.Player_ID WHERE Game_Record.Game_ID = %d", game_id);
			
			// Execute the query
			int err = mysql_query(conn, select_query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Store results
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Process results
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Initialize the response string
			while ((row = mysql_fetch_row(result)) != NULL) {
				if (row[0] != NULL) {
					strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1);
					strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1);
				}
			}
			
			// Free results
			mysql_free_result(result);
			
			// Print the final response
			printf("Player IDs for Game ID %d:\n%s", game_id, respuesta);
		}
		
		else if (codigo == 4) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: See points of a game                                                                               |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Retrieves the points scored by a player in a specific game.                                  |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 4/Game_ID                                                          			                       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Includes in 'respuesta' the points scored in the specified game.                                       |
		// |-----------------------------------------------------------------------------------------------------------|
			int game_id = atoi(p);
			
			// Prepare the query
			char query[256];
			snprintf(query, sizeof(query), "SELECT Points FROM Game_Record WHERE Game_ID = %d", game_id);
			
			// Execute the query
			int err = mysql_query(conn, query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Store results
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Process results
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Initialize the response string
			while ((row = mysql_fetch_row(result)) != NULL) {
				if (row[0] != NULL) {
					strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1);
					strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1);
				}
			}
			
			// Free results
			mysql_free_result(result);
			
			// Print the final response
			printf("Points:\n%s", respuesta);
		}
		
		else if (codigo == 5) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: See games played by a player                                                	                   |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Retrieves the games played by a specific player.    					                       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 5/Player_ID                                                                                        |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Introduces in 'respuesta' the games played by the specified player.   		                           |
		// |-----------------------------------------------------------------------------------------------------------|
			int player_id = atoi(p);
			
			// Prepare the query
			char query[256];
			snprintf(query, sizeof(query), "SELECT Game_ID FROM Game_Record WHERE Player_ID = %d", player_id);
			
			// Execute the query
			int err = mysql_query(conn, query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Store results
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Process results
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Initialize the response string
			while ((row = mysql_fetch_row(result)) != NULL) {
				if (row[0] != NULL) {
					strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1);
					strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1);
				}
			}			
			
			// Free results
			mysql_free_result(result);
			
			// Print the final response
			printf("Played Games:\n%s", respuesta);
		}
		
		else if (codigo == 7) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Show list of clients                                                                               |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Retrieves the list of clients.							     							   |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 7/                                                                                                 |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Introduces in 'respuesta' the list of connected clients.                                               |
		// |-----------------------------------------------------------------------------------------------------------|
			pthread_mutex_lock( &mutex );
			char misConectados[512];
			DameConectados(&miLista, misConectados, username);
			if (misConectados[0] != '\0') {
				strcpy(respuesta, misConectados);
			}
			else {
				strcpy(respuesta, "You are the only player!");
			}
			printf("%s\n", respuesta);
			pthread_mutex_unlock( &mutex);
		}
		
		else if (codigo == 8) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Count players in a game                                                                            |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Checks how many players are participating in a specific game.					               |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 8/Game_ID                                                                                          |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sets 'respuesta' to 1 if 2 <= number of players <= 4.	                                               |
		// |  - Sets 'respuesta' to 0 otherwise.								     								   |
		// |-----------------------------------------------------------------------------------------------------------|
			int game_id = atoi(p);
			
			// Prepare the query
			char query[256];
			snprintf(query, sizeof(query), "SELECT COUNT(Player_ID) FROM Game_Record WHERE Game_ID = %d", game_id);
			
			// Execute the query
			int err = mysql_query(conn, query);
			if (err != 0) {
				printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Store results
			MYSQL_RES *result = mysql_store_result(conn);
			if (result == NULL) {
				printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			
			// Process results
			MYSQL_ROW row;
			respuesta[0] = '\0'; // Initialize the response string
			if ((row = mysql_fetch_row(result)) != NULL) {
				if (row[0] != NULL) {
					int num_players = atoi(row[0]);
					if (num_players >= 2 && num_players <= 4) {
						snprintf(respuesta, sizeof(respuesta), "1\n");
					} 
					else {
						snprintf(respuesta, sizeof(respuesta), "0\n");
					}
				}
			}
			
			// Free results
			mysql_free_result(result);
			
			// Print the final response
			printf("%s", respuesta);
		}
		
		else if (codigo == 9) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Generate random dice rolls                                                                         |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Generates two random integers simulating dice rolls, with each number ranging from 1 to 6.   |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 9/                                                                                                 |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Introduces in 'respuesta' the results of the dice rolls (to random numbers).                           |
		// |-----------------------------------------------------------------------------------------------------------|
			srand(time(NULL));
			
			dice1 = (rand() % 6) + 1;  // First dice
			dice2 = (rand() % 6) + 1;  // Second dice

			snprintf(respuesta, sizeof(respuesta), "Dice rolls: %d, %d\n", dice1, dice2);
			
			// Print the final response
			printf("%s", respuesta);
		}
		
		else if (codigo == 10) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Determine the player with the highest score                                                        |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Parses a series of scores and player IDs, identifying the player with the highest score.     |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 10/Points(i)/Player_ID(i) --> 1 <= i <= 4                                                          |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Introduces in 'respuesta' the player ID and points of the player with the highest score.               |
		// |-----------------------------------------------------------------------------------------------------------|
			int max_score = -1;
			int max_player = -1;
			int current_score, player_id;

			for (int i = 0; i < 4 && *p != '\0'; i++) {
				current_score = atoi(p);
				while (*p != '/' && *p != '\0') p++;
				if (*p == '/') p++;
				
				player_id = atoi(p);
				while (*p != '/' && *p != '\0') p++;
				if (*p == '/') p++;
				
				if (current_score > max_score) {
					max_score = current_score;
					max_player = player_id;
				}
			}
			
			if (max_player != -1) {
				snprintf(respuesta, sizeof(respuesta), "Player with highest score: %d (Score: %d)\n", max_player, max_score);
			} else {
				snprintf(respuesta, sizeof(respuesta), "No valid players found.\n");
			}
			
			// Print the final response
			printf("%s", respuesta);
		}
		
		else if (codigo == 50) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Update token's position                                                                            |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Receives the token's position (1/X/Y/Z/N), where the first number determines the color and   |
		// |              the remaining numbers are used to create a response with the sum of two random dice rolls.   |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input: 50/1/X/Y/Z/N                                                                                       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sends a response in the format of Color/X+i/Y+i/Z+i/N+i where i is the sum of the two dice rolls.      |
		// |-----------------------------------------------------------------------------------------------------------|			
			char* color;
			
			if (strcmp(nombre, "1") == 0) {
				color = "Red";
			} else if (strcmp(nombre, "2") == 0) {
				color = "Blue";
			} else if (strcmp(nombre, "3") == 0) {
				color = "Green";
			} else if (strcmp(nombre, "4") == 0) {
				color = "Yellow";
			} else {
				color = "Invalid";
			}
			
			char* x = strtok(NULL, "/");
			char* y = strtok(NULL, "/");
			char* z = strtok(NULL, "/");
			char* n = strtok(NULL, "/");
			
			int sum = dice1 + dice2;
			
			snprintf(respuesta, sizeof(respuesta), "%s/%d/%d/%d/%d", color, atoi(x) + sum, atoi(y) + sum, atoi(z) + sum, atoi(n) + sum);
			
			// Print the final response
			printf("%s", respuesta);
		}

		else if (codigo == 100) {
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Broadcast a message to all connected clients                                                       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: Handles broadcasting a message from the sender to all connected clients.  	    	       |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input:                                                                                                    |
		// |  - p: A pointer used to parse the incoming command string.                                                |
		// |  - mensaje: A temporary buffer to store the incoming message.                                             |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sends a message to all connected clients.     		                                                   |
		// |-----------------------------------------------------------------------------------------------------------|
			char mensaje[100];
			p = strtok(NULL, "/");
			strcpy(mensaje, p);
			printf("Mensaje a todos: %s\n", mensaje);
			
			char mensaje_final[512];
			sprintf(mensaje_final, "%s: %s\n", nombre, mensaje);
			printf("Mensaje final: %s\n", mensaje_final);
			sprintf(respuesta, mensaje_final);
			EnviarATodos(&miLista, mensaje_final, sock_conn);
		} 
		
/*		else if (codigo == 101) {*/
		// |-----------------------------------------------------------------------------------------------------------|
		// | Query: Send a private message to a specified user                                                         |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Description: This block of code handles sending a private message from the sender to a specific user.     |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Input:                                                                                                    |
		// |  - p: A pointer used to parse the incoming command string.                                                |
		// |  - receptor: A buffer to store the name of the user receiving the message.                                |
		// |  - mensaje: A buffer to hold the message extracted from the command.                                      |
		// |-----------------------------------------------------------------------------------------------------------|
		// | Output:                                                                                                   |
		// |  - Sends a private message to the specified user.                                                         |
		// |-----------------------------------------------------------------------------------------------------------|
/*			char mensaje[100];*/
/*			char receptor[25];*/
/*			p = strtok(NULL, "/");*/
/*			strcpy(receptor, p);*/
/*			p = strtok(NULL, "/");*/
/*			strcpy(mensaje, p);*/

/*			char mensaje_final[512];*/
/*			sprintf(mensaje_final, "%s (privado): %s\n", nombre, mensaje);*/
			
/*			EnviarPrivado(&miLista, receptor, mensaje_final);*/
/*		}*/
		
		if (codigo != 0) {
			// Send response to the client
			write(sock_conn, respuesta, strlen(respuesta));
		}
	}
	
	// Close connection for this client
	close(sock_conn);
	free(s);
}


// MAIN
int main(int argc, char *argv[])
{
    // Create the socket
    int sock_conn, sock_listen;
    struct sockaddr_in serv_adr;
    

    // INITIALIZATIONS
    // Open the socket
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
        printf("Error creating socket");
    
    // Bind to the port
    memset(&serv_adr, 0, sizeof(serv_adr));  // Initialize serv_addr to zero
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);  // Listen on any IP address
    serv_adr.sin_port = htons(9080);  // Listen on port 9080
    
    if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
        printf ("Error on bind");

    // The queue of pending requests cannot exceed 4
    if (listen(sock_listen, 4) < 0)
        printf("Error on listen");	
	
	i = 0;
	pthread_t thread;
    
    for (;;) {
        printf ("Listening\n");

        sock_conn = accept(sock_listen, NULL, NULL);
        printf ("Received a connection\n");
		
		int *sock_conn_ptr = malloc(sizeof(int));
		*sock_conn_ptr = sock_conn;
		
		pthread_create(&thread, NULL, AtenderCliente, sock_conn_ptr);
						
		i = i+1;   
    }
}
