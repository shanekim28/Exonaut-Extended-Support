import java.util.Arrays;
import java.util.List;
import java.util.concurrent.atomic.AtomicInteger;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSUserVariable;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.exceptions.SFSCreateRoomException;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;

// Max 8 players per room
public class FindRoomHandler extends BaseClientRequestHandler {
	private final AtomicInteger roomId = new AtomicInteger();

	@Override
	public void handleClientRequest(User sender, ISFSObject object) {
		List<Room> roomList = getParentExtension().getParentZone().getRoomList();
		Room roomToJoin = null;

		// Loop through available rooms
		for (Room room : roomList) {
			if (room.isFull()) {
				continue;
			} else {
				roomToJoin = room;
				break;
			}
		}

		// If there are no available rooms, create one
		if (roomToJoin == null) {
			try {
				roomToJoin = createNewRoom(sender);
			} catch (SFSCreateRoomException e) {
				trace("Could not join room. Reason: ");
				trace(ExtensionLogLevel.ERROR, e.toString());
			}
		}

		// Try to join room
		try {
			getApi().joinRoom(sender, roomToJoin);

			// Set user variables
			UserVariable nick = new SFSUserVariable("nickName", sender.getName());
			UserVariable level = new SFSUserVariable("level", 1);
			getApi().setUserVariables(sender, Arrays.asList(nick, level));

			getApi().setRoomVariables(null, roomToJoin, Arrays.asList(new SFSRoomVariable("state", "play")));

		} catch (Exception e) {
			trace(ExtensionLogLevel.ERROR, e.toString());
		}
	}

	private Room createNewRoom(User owner) throws SFSCreateRoomException {
		Room room = null;
		var roomSettings = ExonautExtension.ffaRoomSettings;
		roomSettings.setName("Room_" + roomId.getAndIncrement());

		room = getApi().createRoom(getParentExtension().getParentZone(), roomSettings, owner);
		getApi().setRoomVariables(null, room, ExonautExtension.roomVariables);

		return room;
	}
}