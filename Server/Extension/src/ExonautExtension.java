import java.util.ArrayList;
import java.util.List;

import com.smartfoxserver.v2.api.CreateRoomSettings;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.extensions.SFSExtension;

public class ExonautExtension extends SFSExtension {
	public static CreateRoomSettings ffaRoomSettings = new CreateRoomSettings();
	public static List<RoomVariable> roomVariables = new ArrayList<RoomVariable>();
	public static List<UserVariable> userVariables = new ArrayList<UserVariable>();

	@Override
	public void init() {
		createRoomSettings();
		createRoomVariables();
		addRequestHandlers();
	}

	private void addRequestHandlers() {
		addEventHandler(SFSEventType.USER_LOGIN, LoginEventHandler.class);

		addRequestHandler("evt", EventHandler.class);
		addRequestHandler("findRoom", FindRoomHandler.class);
	}

	private void createRoomVariables() {
		// Create room variables
		roomVariables.add(new SFSRoomVariable("stop", null));
		roomVariables.add(new SFSRoomVariable("mapId", 0));
		roomVariables.add(new SFSRoomVariable("state", "wait_for_min_players"));
		roomVariables.add(new SFSRoomVariable("hackLimit", 50));
	}

	private void createRoomSettings() {
		// Create ffa room settings
		ffaRoomSettings.setMaxUsers(8);
		ffaRoomSettings.setDynamic(true);
		ffaRoomSettings.setGame(true);
	}
}
