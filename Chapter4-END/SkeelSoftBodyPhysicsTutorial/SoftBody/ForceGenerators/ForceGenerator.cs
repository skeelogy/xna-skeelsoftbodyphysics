using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators
{
    public interface ForceGenerator
    {
        void ApplyForce(SimObject simObject);
    }
}
