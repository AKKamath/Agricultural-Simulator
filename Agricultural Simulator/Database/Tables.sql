SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

-- --------------------------------------------------------

--
-- Table structure for table `Inventory`
--

CREATE TABLE `Inventory` (
  `UserId` int(11) NOT NULL,
  `ItemId` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Item`
--

CREATE TABLE `Item` (
  `Id` int(11) NOT NULL,
  `Name` varchar(40) COLLATE utf8_unicode_ci NOT NULL,
  `Price` int(11) NOT NULL,
  `Stock` int(11) NOT NULL,
  `Action` int(11) NOT NULL,
  `Link` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `Fact` varchar(200) COLLATE utf8_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Seed`
--

CREATE TABLE `Seed` (
  `ItemId` int(11) NOT NULL,
  `GrowTime` datetime NOT NULL,
  `Water` int(11) NOT NULL,
  `Produce` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Tasks`
--

CREATE TABLE `Tasks` (
  `UserId` int(11) NOT NULL,
  `TaskId` int(11) NOT NULL,
  `Start_time` datetime NOT NULL,
  `Completion_time` datetime NOT NULL,
  `Fail_time` datetime NOT NULL,
  `Update_time` datetime NOT NULL,
  `Type` int(11) NOT NULL,
  `TileId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Tile`
--

CREATE TABLE `Tile` (
  `UserId` int(11) NOT NULL,
  `TileId` int(11) NOT NULL,
  `X` int(11) NOT NULL,
  `Y` int(11) NOT NULL,
  `Value` int(11) NOT NULL,
  `Type` int(11) NOT NULL,
  `Parent` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `User`
--

CREATE TABLE `User` (
  `Id` int(11) NOT NULL,
  `Username` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `Password` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `Join_date` date NOT NULL,
  `Currency` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Indexes for table `Inventory`
--
ALTER TABLE `Inventory`
  ADD PRIMARY KEY (`ItemId`),
  ADD KEY `UserId` (`UserId`);

--
-- Indexes for table `Item`
--
ALTER TABLE `Item`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `Seed`
--
ALTER TABLE `Seed`
  ADD PRIMARY KEY (`ItemId`),
  ADD KEY `Produce` (`Produce`);

--
-- Indexes for table `Tasks`
--
ALTER TABLE `Tasks`
  ADD PRIMARY KEY (`TaskId`),
  ADD KEY `Tasks_ibfk_1` (`UserId`),
  ADD KEY `Tasks_ibfk_2` (`TileId`);

--
-- Indexes for table `Tile`
--
ALTER TABLE `Tile`
  ADD PRIMARY KEY (`TileId`),
  ADD KEY `UserId` (`UserId`);

--
-- Indexes for table `User`
--
ALTER TABLE `User`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Username` (`Username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Inventory`
--
ALTER TABLE `Inventory`
  MODIFY `ItemId` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `Item`
--
ALTER TABLE `Item`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `Tasks`
--
ALTER TABLE `Tasks`
  MODIFY `TaskId` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `Tile`
--
ALTER TABLE `Tile`
  MODIFY `TileId` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `User`
--
ALTER TABLE `User`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `Inventory`
--
ALTER TABLE `Inventory`
  ADD CONSTRAINT `Inventory_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `Seed`
--
ALTER TABLE `Seed`
  ADD CONSTRAINT `Seed_ibfk_1` FOREIGN KEY (`ItemId`) REFERENCES `Item` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Seed_ibfk_2` FOREIGN KEY (`Produce`) REFERENCES `Item` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Constraints for table `Tasks`
--
ALTER TABLE `Tasks`
  ADD CONSTRAINT `Tasks_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `Tasks_ibfk_2` FOREIGN KEY (`TileId`) REFERENCES `Tile` (`TileId`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `Tile`
--
ALTER TABLE `Tile`
  ADD CONSTRAINT `Tile_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

