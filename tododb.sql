-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 28, 2025 at 01:33 PM
-- Wersja serwera: 10.4.32-MariaDB
-- Wersja PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `tododb`
--

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `tasks`
--

CREATE TABLE `tasks` (
  `ID` int(11) NOT NULL,
  `Expiry` datetime DEFAULT NULL,
  `Title` varchar(30) NOT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CompletePercentage` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tasks`
--

INSERT INTO `tasks` (`ID`, `Expiry`, `Title`, `Description`, `CompletePercentage`) VALUES
(2, '2025-04-25 01:02:00', 'Title tomorrow(25)', 'description iwau dpiabd iabd pabwd a', 100),
(3, '2025-04-27 01:02:00', 'Title (27)', 'description iwau dpiabd iabd pabwd a', 100),
(4, '2025-04-26 01:02:00', 'Title tomorrow(26)', 'description iwau dpiabd iabd pabwd a', 20),
(5, '2024-04-25 01:02:00', 'Title (2024)', 'description iwau dpiabd iabd pabwd a', 0),
(6, '2025-04-29 10:43:33', 'post created by post', 'updated by put', 50),
(7, '2025-04-24 12:31:04', 'No desc', NULL, 0);

--
-- Indeksy dla zrzutów tabel
--

--
-- Indeksy dla tabeli `tasks`
--
ALTER TABLE `tasks`
  ADD PRIMARY KEY (`ID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tasks`
--
ALTER TABLE `tasks`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
