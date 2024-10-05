#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>

int main(int argc, char *argv[])
{
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

    // Create the socket
    int sock_conn, sock_listen, ret;
    struct sockaddr_in serv_adr;
    char peticion[512];
    char respuesta[512];

    // INITIALIZATIONS
    // Open the socket
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
        printf("Error creating socket");
    
    // Bind to the port
    memset(&serv_adr, 0, sizeof(serv_adr));  // Initialize serv_addr to zero
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);  // Listen on any IP address
    serv_adr.sin_port = htons(4247);  // Listen on port 9080
    
    if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
        printf ("Error on bind");

    // The queue of pending requests cannot exceed 4
    if (listen(sock_listen, 4) < 0)
        printf("Error on listen");

    int i;
    
    while (1>0) {
        printf ("Listening\n");

        sock_conn = accept(sock_listen, NULL, NULL);
        printf ("Received a connection\n");
        
        // Loop for client communication
        int terminar = 0;
        while (terminar == 0) {
            // Receive request
            ret = read(sock_conn, peticion, sizeof(peticion));
            printf ("Received a request\n");
            
            // Null-terminate the request string
            peticion[ret] = '\0';
            printf ("The request is: %s\n", peticion);

            char *p = strtok(peticion, "/");
            int codigo = atoi(p);  // Extract code

            char nombre[25];
            char password[25];

            if (codigo != 0) {
                p = strtok(NULL, "/");
                strcpy(nombre, p);
                printf("Code: %d, Name: %s\n", codigo, nombre);
            }

            if (codigo == 0) {
                terminar = 1;
            } 
			
			else if (codigo == 1) {  // Register  --> 1/Username/Password
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
				respuesta[0] = '\0';
					
            } 
			
			else if (codigo == 2) {  // Log in --> 2/Username/Password
				p = strtok(NULL, "/");
				strcpy(password, p);
				
				// Check if the user exists and password matches
				char query[256];
				sprintf(query, "SELECT Username FROM Users WHERE Username='%s' AND Password='%s'", nombre, password);
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
					sprintf(respuesta, "2");  // Login successful
				}
            }
			
			else if (codigo == 3) { // Select players of a game --> 3/Player_ID/Game
				int game_id = atoi(p);
				
				// Preparar la consulta para seleccionar
				char select_query[256];
				snprintf(select_query, sizeof(select_query), "SELECT DISTINCT Users.Username FROM Users JOIN Game_Record ON Users.Player_ID = Game_Record.Player_ID WHERE Game_Record.Game_ID = %d", game_id);
				
				// Ejecutar la consulta de selección
				int err = mysql_query(conn, select_query);
				if (err != 0) {
					printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				
				// Almacenar resultados
				MYSQL_RES *result = mysql_store_result(conn);
				if (result == NULL) {
					printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				
				
					
				// Procesar resultados
				MYSQL_ROW row;
				respuesta[0] = '\0'; // Inicializa la cadena de respuesta
				while ((row = mysql_fetch_row(result)) != NULL) {
					if (row[0] != NULL) {
						// Añadir Player_ID a la respuesta
						strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1);
						strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1);
					}
				}
				
				// Liberar resultados
				mysql_free_result(result);
				
				// Imprimir la respuesta final
				printf("Player IDs for Game ID %d:\n%s", game_id, respuesta);

			}
				
			else if (codigo == 4) { // See points of a game --> 4/Game
				int game_id = atoi(p);
				
				// Preparar la consulta
				char query[256];
				snprintf(query, sizeof(query), "SELECT Points FROM Game_Record WHERE Game_ID = %d", game_id);
				
				// Ejecutar la consulta
				int err = mysql_query(conn, query);
				if (err != 0) {
					printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				
				// Almacenar resultados
				MYSQL_RES *result = mysql_store_result(conn);
				if (result == NULL) {
					printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				
				// Procesar resultados
				MYSQL_ROW row;
				respuesta[0] = '\0'; // Inicializa la cadena de respuesta
				while ((row = mysql_fetch_row(result)) != NULL) {
					// Verificar que row[0] no sea NULL
					if (row[0] != NULL) {
						// Añadir los puntos a la respuesta
						strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1); // Añadir puntos
						strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1); // Añadir nueva línea
					}
				}
				
				// Liberar resultados
				mysql_free_result(result);
				
				// Imprimir la respuesta final
				printf("Points:\n%s", respuesta);
			}
			
			else if (codigo == 5) { // See games played by a player --> 5/Player
				int player_id = atoi(p);
				
				// Preparar la consulta
				char query[256];
				snprintf(query, sizeof(query), "SELECT Game_ID FROM Game_Record WHERE Player_ID = %d", player_id);
				
				// Ejecutar la consulta
				int err = mysql_query(conn, query);
				if (err != 0) {
					printf("Error querying data from database: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				
				// Almacenar resultados
				MYSQL_RES *result = mysql_store_result(conn);
				if (result == NULL) {
					printf("Error retrieving results: %u %s\n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
				
				// Procesar resultados
				MYSQL_ROW row;
				respuesta[0] = '\0'; // Inicializa la cadena de respuesta
				while ((row = mysql_fetch_row(result)) != NULL) {
					// Verificar que row[0] no sea NULL
					if (row[0] != NULL) {
						// Añadir los puntos a la respuesta
						strncat(respuesta, row[0], sizeof(respuesta) - strlen(respuesta) - 1); // Añadir puntos
						strncat(respuesta, "\n", sizeof(respuesta) - strlen(respuesta) - 1); // Añadir nueva línea
					}
				}
				
				// Liberar resultados
				mysql_free_result(result);
				
				// Imprimir la respuesta final
				printf("Played Games:\n%s", respuesta);
			}
				

            if (codigo != 0) {
                // Send response to client
                write(sock_conn, respuesta, strlen(respuesta));
            }
        }

        // Close connection for this client
        close(sock_conn);
    }
}
