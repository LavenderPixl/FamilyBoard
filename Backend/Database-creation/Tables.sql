-- DROP DATABASE IF EXISTS familyBoard;
-- CREATE DATABASE familyBoard;
\c familyBoard

CREATE TABLE families(
    id SERIAL PRIMARY KEY,
    name VARCHAR(32) 
);

CREATE TABLE familyCodes(
    id SERIAL primary key,
    code CHAR(8) NOT NULL ,
    expiration date NOT NULL,
    family_id INT,
    
    CONSTRAINT fk_family_codes FOREIGN KEY(family_id) REFERENCES families(id) ON DELETE CASCADE 
);

CREATE TABLE users(
    id SERIAL PRIMARY KEY,
    username VARCHAR(32) NOT NULL,
    email VARCHAR(320) NOT NULL UNIQUE,
    hashed_password VARCHAR NOT NULL,
    points INT NOT NULL DEFAULT 0,
    is_adult BOOLEAN NOT NULL DEFAULT FALSE,
    family_id INT,
    
    CONSTRAINT fk_family_users FOREIGN KEY(family_id) REFERENCES families(id) ON DELETE SET NULL
);

CREATE TABLE tasks(
    id SERIAL PRIMARY KEY,
    task VARCHAR(128) NOT NULL,
    reward INT NOT NULL,
    completed BOOLEAN NOT NULL DEFAULT FALSE,
    family_id INT NOT NULL,
    user_id INT,

    CONSTRAINT fk_family FOREIGN KEY(family_id) REFERENCES families(id) ON DELETE CASCADE,
    CONSTRAINT fk_user FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE SET NULL
);

CREATE TABLE repeatingTasks(
    id SERIAL PRIMARY KEY,
    task VARCHAR(128) NOT NULL,
    reward INT NOT NULL,
    completed BOOLEAN NOT NULL DEFAULT FALSE,
    next_date DATE NOT NULL,
    interval_days INT NOT NULL,
    family_id INT NOT NULL,
    user_id INT,

    CONSTRAINT fk_family FOREIGN KEY(family_id) REFERENCES families(id) ON DELETE CASCADE,
    CONSTRAINT fk_user FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE goals(
    id SERIAL PRIMARY KEY,
    goal VARCHAR(128) NOT NULL,
    progress INT NOT NULL DEFAULT 0,
    cost INT NOT NULL,
    user_id INT NOT NULL,
    
    CONSTRAINT fk_family FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE 
)