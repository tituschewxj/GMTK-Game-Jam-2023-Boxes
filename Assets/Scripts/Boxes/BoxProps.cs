// Stores the list of properties that a box type can have
public class BoxProps
{
    public bool isControllable, // Whether the box is a box that can be moved by the controller input.
        isBlockable, // Whether the box can block the player.
        isPushable, // Whether the box can be moved by the player.
        isStationary, // Whether the box is stationary: so is stored on a static grid.
        isIgnored, // Ignored boxes are not added to the grid, just a visual thing.
        isInteractable; // If the box is not blockable, and can be interacted with: triggers something when entered.

    BoxProps(bool isControllable = false, 
        bool isBlockable = false, 
        bool isPushable = false, 
        bool isStationary = false,
        bool isIgnored = false,
        bool isInteractable = false) {

        this.isControllable = isControllable;
        this.isBlockable = isBlockable;
        this.isPushable = isPushable;
        this.isStationary = isStationary;
        this.isIgnored = isIgnored;
        this.isInteractable = isInteractable;
    }

    public static BoxProps Empty() {
        return new BoxProps();
    }
    public static BoxProps BoxControllable() {
        return new BoxProps(isControllable: true, isPushable: true, isBlockable: true);
    }
    public static BoxProps BoxUncontrollable() {
        return new BoxProps(isPushable: true, isBlockable: true);
    }
    public static BoxProps Player() {
        return new BoxProps(isBlockable: true);
    }
    public static BoxProps Wall() {
        return new BoxProps(isStationary: true, isBlockable: true);
    }
    public static BoxProps OpenedDoor() {
        return new BoxProps(isStationary: true);
    }
    public static BoxProps ClosedDoor() {
        return new BoxProps(isStationary: true, isBlockable: true);
    }
    public static BoxProps Goal() {
        return new BoxProps(isStationary: true);
    }
    public static BoxProps Sublevel() {
        return new BoxProps(isStationary: true);
    }
    public static BoxProps TrapPlayer() {
        return new BoxProps(isStationary: true);
    }
    public static BoxProps Trap() {
        return new BoxProps(isStationary: true, isInteractable: true);
    }
    public static BoxProps Button() {
        return new BoxProps(isStationary: true, isInteractable: true);
    }
    public static BoxProps Cursor() {
        return new BoxProps(isIgnored: true);
    }
}
