--
-- Скрипт сгенерирован Devart dbForge Studio 2019 for MySQL, Версия 8.1.45.0
-- Домашняя страница продукта: http://www.devart.com/ru/dbforge/mysql/studio
-- Дата скрипта: 17.06.2019 11:46:17
-- Версия сервера: 5.5.5-10.1.38-MariaDB
-- Версия клиента: 4.1
--

-- 
-- Отключение внешних ключей
-- 
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;

-- 
-- Установить режим SQL (SQL mode)
-- 
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 
-- Установка кодировки, с использованием которой клиент будет посылать запросы на сервер
--
SET NAMES 'utf8';

--
-- Удалить таблицу `user_friend`
--
DROP TABLE IF EXISTS user_friend;

--
-- Удалить таблицу `users`
--
DROP TABLE IF EXISTS users;

--
-- Создать таблицу `users`
--
CREATE TABLE users (
  id_user bigint(20) NOT NULL AUTO_INCREMENT,
  login varchar(255) NOT NULL,
  password varchar(255) NOT NULL,
  email varchar(255) NOT NULL,
  register_id varchar(255) NOT NULL,
  permission int(1) DEFAULT NULL,
  PRIMARY KEY (id_user)
)
ENGINE = INNODB,
AUTO_INCREMENT = 14,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

--
-- Создать таблицу `user_friend`
--
CREATE TABLE user_friend (
  id_user_friend bigint(20) NOT NULL AUTO_INCREMENT,
  id_user bigint(20) NOT NULL,
  id_friend bigint(20) NOT NULL,
  PRIMARY KEY (id_user_friend)
)
ENGINE = INNODB,
AUTO_INCREMENT = 34,
AVG_ROW_LENGTH = 2730,
CHARACTER SET utf8,
COLLATE utf8_general_ci;

--
-- Создать внешний ключ
--
ALTER TABLE user_friend
ADD CONSTRAINT user_friend_ibfk_1 FOREIGN KEY (id_user)
REFERENCES users (id_user) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Создать внешний ключ
--
ALTER TABLE user_friend
ADD CONSTRAINT user_friend_ibfk_2 FOREIGN KEY (id_friend)
REFERENCES users (id_user) ON DELETE CASCADE ON UPDATE CASCADE;

-- 
-- Вывод данных для таблицы users
--
INSERT INTO users VALUES
(1, 'vlad', '245c15cd5377a0c555a0b4d0f5e20b86', 'vladdaking@gmail.com', '', 0),
(2, 'ezhik', '245c15cd5377a0c555a0b4d0f5e20b86', 'ezhik1234@mail.ru', '', 0),
(9, 'vlad222', '245c15cd5377a0c555a0b4d0f5e20b86', 'vladdaking3@gmail.com', '', 0),
(11, 'belka', '245c15cd5377a0c555a0b4d0f5e20b86', 'vladdaking4@gmail.com', '', 0),
(12, 'zayka', '245c15cd5377a0c555a0b4d0f5e20b86', 'vladdaking5@gmail.com', '', 0),
(13, 'tigr', '245c15cd5377a0c555a0b4d0f5e20b86', 'vladdaking2@gmail.com', '', 0);

-- 
-- Вывод данных для таблицы user_friend
--
INSERT INTO user_friend VALUES
(26, 1, 2),
(27, 2, 1),
(28, 11, 12),
(29, 12, 11),
(32, 11, 13),
(33, 13, 11);

-- 
-- Восстановить предыдущий режим SQL (SQL mode)
-- 
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;

-- 
-- Включение внешних ключей
-- 
/*!40014 SET FOREIGN_KEY_CHECKS = @OLD_FOREIGN_KEY_CHECKS */;