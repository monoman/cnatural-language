// See: http://shootout.alioth.debian.org/

using java.lang;

public class nbody {
    public static bool test(int n, string v1, string v2) {
        var bodies = new NBodySystem();
        if (!v1.equals("" + bodies.energy()))
        	return false;
        for (int i = 0; i < n; ++i)
           bodies.advance(0.01);
        return v2.equals("" + bodies.energy());
    }
}

final class NBodySystem {
   private Body[] bodies;

   public NBodySystem(){
      bodies = new[] {
            Body.sun(),
            Body.jupiter(),
            Body.saturn(),
            Body.uranus(),
            Body.neptune()
         };

      var px = 0.0;
      var py = 0.0;
      var pz = 0.0;
      for (int i = 0; i < sizeof(bodies); ++i) {
         px += bodies[i].vx * bodies[i].mass;
         py += bodies[i].vy * bodies[i].mass;
         pz += bodies[i].vz * bodies[i].mass;
      }
      bodies[0].offsetMomentum(px,py,pz);
   }

   public void advance(double dt) {
      for (int i = 0; i < sizeof(bodies); ++i) {
         var iBody = bodies[i];
         for (int j = i + 1; j < sizeof(bodies); ++j) {
            var dx = iBody.x - bodies[j].x;
            var dy = iBody.y - bodies[j].y;
            var dz = iBody.z - bodies[j].z;

            var dSquared = dx * dx + dy * dy + dz * dz;
            var distance = Math.sqrt(dSquared);
            var mag = dt / (dSquared * distance);

            iBody.vx -= dx * bodies[j].mass * mag;
            iBody.vy -= dy * bodies[j].mass * mag;
            iBody.vz -= dz * bodies[j].mass * mag;

            bodies[j].vx += dx * iBody.mass * mag;
            bodies[j].vy += dy * iBody.mass * mag;
            bodies[j].vz += dz * iBody.mass * mag;
         }
      }

      foreach (var body in bodies) {
         body.x += dt * body.vx;
         body.y += dt * body.vy;
         body.z += dt * body.vz;
      }
   }

   public double energy(){
      double dx, dy, dz, distance;
      var e = 0.0;

      for (int i = 0; i < sizeof(bodies); ++i) {
      	 var iBody = bodies[i];
         e += 0.5 * iBody.mass * (iBody.vx * iBody.vx + iBody.vy * iBody.vy + iBody.vz * iBody.vz );

         for (int j = i + 1; j < sizeof(bodies); ++j) {
            var jBody = bodies[j];
            dx = iBody.x - jBody.x;
            dy = iBody.y - jBody.y;
            dz = iBody.z - jBody.z;

            distance = Math.sqrt(dx*dx + dy*dy + dz*dz);
            e -= (iBody.mass * jBody.mass) / distance;
         }
      }
      return e;
   }
}


final class Body {
   static final double PI = 3.141592653589793;
   static final double SOLAR_MASS = 4 * PI * PI;
   static final double DAYS_PER_YEAR = 365.24;

   public double x, y, z, vx, vy, vz, mass;

   public Body(){}

   static Body jupiter(){
      return new Body {
         x = 4.84143144246472090e+00,
         y = -1.16032004402742839e+00,
         z = -1.03622044471123109e-01,
         vx = 1.66007664274403694e-03 * DAYS_PER_YEAR,
         vy = 7.69901118419740425e-03 * DAYS_PER_YEAR,
         vz = -6.90460016972063023e-05 * DAYS_PER_YEAR,
         mass = 9.54791938424326609e-04 * SOLAR_MASS
      };
   }

   static Body saturn(){
      return new Body {
         x = 8.34336671824457987e+00,
         y = 4.12479856412430479e+00,
         z = -4.03523417114321381e-01,
         vx = -2.76742510726862411e-03 * DAYS_PER_YEAR,
         vy = 4.99852801234917238e-03 * DAYS_PER_YEAR,
         vz = 2.30417297573763929e-05 * DAYS_PER_YEAR,
         mass = 2.85885980666130812e-04 * SOLAR_MASS
      };
   }

   static Body uranus(){
      return new Body {
         x = 1.28943695621391310e+01,
         y = -1.51111514016986312e+01,
         z = -2.23307578892655734e-01,
         vx = 2.96460137564761618e-03 * DAYS_PER_YEAR,
         vy = 2.37847173959480950e-03 * DAYS_PER_YEAR,
         vz = -2.96589568540237556e-05 * DAYS_PER_YEAR,
         mass = 4.36624404335156298e-05 * SOLAR_MASS
      };
   }

   static Body neptune(){
      return new Body {
         x = 1.53796971148509165e+01,
         y = -2.59193146099879641e+01,
         z = 1.79258772950371181e-01,
         vx = 2.68067772490389322e-03 * DAYS_PER_YEAR,
         vy = 1.62824170038242295e-03 * DAYS_PER_YEAR,
         vz = -9.51592254519715870e-05 * DAYS_PER_YEAR,
         mass = 5.15138902046611451e-05 * SOLAR_MASS
      };
   }

   static Body sun(){
      return new Body {
         mass = SOLAR_MASS
      };
   }

   Body offsetMomentum(double px, double py, double pz){
      vx = -px / SOLAR_MASS;
      vy = -py / SOLAR_MASS;
      vz = -pz / SOLAR_MASS;
      return this;
   }
}
