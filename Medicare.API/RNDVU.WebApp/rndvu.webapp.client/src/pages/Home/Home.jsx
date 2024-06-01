import { useSelector } from "react-redux";
import Tile from "./Tile";
import "./Home.scss";

import { MdToday } from "react-icons/md";
import { GrLinkNext } from "react-icons/gr";
import { TiTickOutline } from "react-icons/ti";
import { MdOutlinePeopleAlt } from "react-icons/md";

function Home() {
    const user = useSelector((state) => state.auth);

    if (!user.dayAppointmentsCount)
        return <div className="spinner-grow home-spinner" role="status"></div>;

    const displayName =
        user?.fullName.length <= 20
            ? user?.fullName
            : user?.fullName.split(" ")[0] || "";
    return (
        <div className="home">
            <h2>Hi, {displayName}! Examine your Medicare activity:</h2>
            <Tile
                color="#075985"
                icon={<MdToday />}
                title="Appointments today"
                value={user?.dayAppointmentsCount.length}
            />
            <Tile
                color="#166534"
                icon={<GrLinkNext />}
                title="Appointments this week"
                value={user?.weekAppointmentsCount.length}
            />
            <Tile
                color="#854D0E"
                icon={<TiTickOutline />}
                title="Total appointments number"
                value={user?.allAppoinementsCount}
            />
            <Tile
                color="#3730A3"
                icon={<MdOutlinePeopleAlt />}
                title={
                    user.isPatient
                        ? "Different doctors contacted"
                        : "Different patients treated"
                }
                value={user?.differentDoctorsOrUsersCount}
            />
        </div>
    );
}

export default Home;
